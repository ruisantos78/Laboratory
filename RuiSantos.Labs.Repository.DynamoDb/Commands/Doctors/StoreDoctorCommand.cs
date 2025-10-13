using System.Data;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using FluentValidation;
using Microsoft.Extensions.Logging;
using RuiSantos.Labs.Domain.Commands;
using RuiSantos.Labs.Domain.Models;
using RuiSantos.Labs.Repository.DynamoDb.Entities;
using RuiSantos.Labs.Repository.DynamoDb.Entities.Converters;

namespace RuiSantos.Labs.Repository.DynamoDb.Commands.Doctors;

internal class StoreDoctorCommand(Doctor doctor
    , IAmazonDynamoDB client
    , ILogger<StoreDoctorCommand> logger
    ): IAsyncCommand
{   
    private readonly DynamoDBContext _context = new DynamoDBContextBuilder()
        .WithDynamoDBClient(() => client)
        .Build(); 
    
    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Storing doctor {DoctorId}", doctor.Id);
        try
        {
            await ValidateAndThrowAsync(cancellationToken);
            
            var entity = doctor.ToDoctorEntity();
            entity.Id = await GetDoctorIdByLicenseAsync(entity.License);

            var doctorWriter = _context.CreateBatchWrite<DoctorEntity>();
            doctorWriter.AddPutItem(entity);
            
            var specsWriter = await CreateSpecializationsWriterAsync(entity, cancellationToken);
            
            await doctorWriter.Combine(specsWriter) 
                .ExecuteAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error storing doctor {DoctorId}", doctor.Id);
            throw;
        }
    }
    
    private async Task<Guid> GetDoctorIdByLicenseAsync(string license)
    {
        var query = new QueryOperationConfig
        {
            IndexName = DoctorEntity.DoctorLicenceIndexName,
            Filter = new QueryFilter(nameof(DoctorEntity.License), QueryOperator.Equal, license),
            Limit = 1,
            Select = SelectValues.SpecificAttributes,
            AttributesToGet = [nameof(DoctorEntity.Id)]
        };

        var result = await _context.FromQueryAsync<DoctorEntity>(query)
            .GetNextSetAsync()
            .ContinueWith(task => task.Result.FirstOrDefault()?.Id);

        return result ?? doctor.Id;
    }

    private async Task<IBatchWrite<DoctorSpecializationEntity>> CreateSpecializationsWriterAsync(DoctorEntity entity, CancellationToken cancellationToken)
    {
        var writer = _context.CreateBatchWrite<DoctorSpecializationEntity>();

        var current = await _context.FromQueryAsync<DoctorSpecializationEntity>(new QueryOperationConfig
        {
            IndexName = DoctorSpecializationEntity.DoctorSpecializationByDoctorIdIndexName,
            Filter = new QueryFilter(nameof(DoctorSpecializationEntity.DoctorId), QueryOperator.Equal, entity.Id),
            Select = SelectValues.SpecificAttributes,
            AttributesToGet = [nameof(DoctorSpecializationEntity.Specialization)]
        }).GetRemainingAsync(cancellationToken);            

        var excludeItems = current.Where(x => !doctor.Specializations.Contains(x.Specialization)).ToArray();        
        var includeItems = doctor.Specializations
            .Except(current.Select(x => x.Specialization))
            .Select(specialization => new DoctorSpecializationEntity
            {
                DoctorId = entity.Id,
                Specialization = specialization
            })
            .ToArray();

        writer.AddDeleteItems(excludeItems);
        writer.AddPutItems(includeItems);
        return writer;
    }
    
    private async Task ValidateAndThrowAsync(CancellationToken cancellationToken)
    {
        var validator = new InlineValidator<Doctor>();

        validator.RuleFor(x => x.License)
            .NotEmpty();

        await validator.ValidateAndThrowAsync(doctor, cancellationToken);
    }    
}