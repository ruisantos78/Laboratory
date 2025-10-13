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

internal class GetDoctorByLicenseCommand(GetDoctorByLicenseCommandInput input
    , IAmazonDynamoDB client
    , ILogger<GetDoctorByIdCommand> logger
): IAsyncCommand<Doctor?>
{
    private readonly DynamoDBContext _context = new DynamoDBContextBuilder()
        .WithDynamoDBClient(() => client)
        .Build();
    
    public async Task<Doctor?> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("GetDoctorByLicenseCommand: {LicenseNumber}", input.LicenseNumber);
        try
        {
            await ValidateAndThrowAsync(cancellationToken);

            var doctorEntity = await _context.GetDoctorByLicenseAsync(input.LicenseNumber, cancellationToken);
            if (doctorEntity is null)
                return null;

            var specializations = await _context.GetDoctorSpecializationsAsync(doctorEntity, cancellationToken);
            return doctorEntity.ToDoctor(specializations);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GetDoctorByLicenseCommand: {LicenseNumber}", input.LicenseNumber);
            throw;
        }
    }
    
    private async Task ValidateAndThrowAsync(CancellationToken cancellationToken)
    {
        var validator = new InlineValidator<GetDoctorByLicenseCommandInput>();
        
        validator.RuleFor(x => x.LicenseNumber)
            .NotEmpty();
        
        await validator.ValidateAndThrowAsync(input, cancellationToken: cancellationToken);
    }
}

internal struct GetDoctorByLicenseCommandInput
{
    public required string LicenseNumber { get; init; }
}