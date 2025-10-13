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
using RuiSantos.Labs.Repository.DynamoDb.Entities.Queries;

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
            entity.Id = await _context.GetDoctorIdByLicenseAsync(entity.License, cancellationToken);

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

    private async Task<IBatchWrite<DoctorSpecializationEntity>> CreateSpecializationsWriterAsync(DoctorEntity doctorEntity, CancellationToken cancellationToken)
    {
        var writer = _context.CreateBatchWrite<DoctorSpecializationEntity>();
        
        await foreach (var spec in _context.GetDoctorSpecializationAsyncEnumerable(doctorEntity, cancellationToken))
        {
            if (doctor.Specializations.Contains(spec.Description))
            {
                doctor.Specializations.Remove(spec.Description);
                continue;
            }
            
            writer.AddDeleteItem(spec);
        }
        
        if (doctor.Specializations.Count == 0)
            return writer;

        writer.AddPutItems(doctor.Specializations.Select(x => new DoctorSpecializationEntity
        {
            DoctorId = doctorEntity.Id,
            Specialization = x.ToLower(),
            Description = x
        }));
        
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