using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using FluentValidation;
using Microsoft.Extensions.Logging;
using RuiSantos.Labs.Domain.Commands;
using RuiSantos.Labs.Domain.Models;
using RuiSantos.Labs.Repository.DynamoDb.Entities;
using RuiSantos.Labs.Repository.DynamoDb.Entities.Converters;
using RuiSantos.Labs.Repository.DynamoDb.Entities.Validators;

namespace RuiSantos.Labs.Repository.DynamoDb.Commands.Doctors;

internal class StoreDoctorCommand(Doctor doctor
    , IAmazonDynamoDB client
    , ILogger<StoreDoctorCommand> logger
    ): IAsyncCommand
{
    private readonly DoctorEntityValidator _validator = new();
    
    private readonly DynamoDBContext _context = new DynamoDBContextBuilder()
        .WithDynamoDBClient(() => client)
        .Build(); 
    
    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Storing doctor {DoctorId}", doctor.Id);
        try
        {
            var entity = doctor.ToDoctorEntity();
            await _validator.ValidateAndThrowAsync(entity, cancellationToken);
            
            if (await GetDoctorIdByLicenseAsync(entity.License) is { } doctorId)
                entity.Id = doctorId;

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
    
    private Task<Guid?> GetDoctorIdByLicenseAsync(string license)
    {
        var query = new QueryOperationConfig
        {
            IndexName = DoctorEntity.DoctorLicenceIndexName,
            Filter = new QueryFilter(nameof(DoctorEntity.License), QueryOperator.Equal, license),
            Limit = 1,
            Select = SelectValues.SpecificAttributes,
            AttributesToGet = [nameof(DoctorEntity.License)]
        };

        return _context.FromQueryAsync<DoctorEntity>(query)
            .GetNextSetAsync()
            .ContinueWith(task => task.Result.FirstOrDefault()?.Id);
    }
    
    private async Task<IBatchWrite<DoctorSpecializationEntity>> CreateSpecializationsWriterAsync(DoctorEntity entity, CancellationToken cancellationToken)
    {
        var writer = _context.CreateBatchWrite<DoctorSpecializationEntity>();

        var query = new QueryOperationConfig
        {
            IndexName = DoctorSpecializationEntity.DoctorSpecializationByDoctorIdIndexName,
            Filter = new QueryFilter(nameof(DoctorSpecializationEntity.DoctorId), QueryOperator.Equal, entity.Id)
        };

        var current = await _context.FromQueryAsync<DoctorSpecializationEntity>(query)
            .GetRemainingAsync(cancellationToken);
        
        writer.AddDeleteItems(current.Where(x => !doctor.Specializations.Contains(x.Specialization)));

        writer.AddPutItems(doctor.Specializations
            .Except(current.Select(x => x.Specialization))
            .Select(specialization => new DoctorSpecializationEntity
            {
                DoctorId = entity.Id,
                Specialization = specialization
            })
        );

        return writer;
    }
}