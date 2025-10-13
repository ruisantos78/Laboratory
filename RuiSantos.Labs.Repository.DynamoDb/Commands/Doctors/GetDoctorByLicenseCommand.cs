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
            
            var query = new QueryOperationConfig
            {
                IndexName = DoctorEntity.DoctorLicenceIndexName,
                Filter = new QueryFilter(nameof(DoctorEntity.License), QueryOperator.Equal, input.LicenseNumber),
                Limit = 1
            };

            var doctorEntity = await _context.FromQueryAsync<DoctorEntity>(query)
                .GetNextSetAsync(cancellationToken)
                .ContinueWith(task => task.Result.FirstOrDefault(), cancellationToken);
            
            if (doctorEntity is null)
                return null;

            string[] specializations = [..
                await _context.QueryAsync<DoctorSpecializationEntity>(doctorEntity.Id)
                    .GetRemainingAsync(cancellationToken)
                    .ContinueWith(t => t.Result.Select(x => x.Specialization), cancellationToken)
            ];

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