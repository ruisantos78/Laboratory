using System.Runtime.CompilerServices;
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

internal class GetDoctorsBySpecializationCommand(GetDoctorsBySpecializationCommandInput input
    , IAmazonDynamoDB client
    , ILogger<GetDoctorsBySpecializationCommand> logger
    ): IAsyncCommand<IReadOnlyList<Doctor>>
{
    private readonly DynamoDBContext _context = new DynamoDBContextBuilder()
        .WithDynamoDBClient(() => client)
        .Build();

    public async Task<IReadOnlyList<Doctor>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("GetDoctorsBySpecializationCommand.ExecuteAsync");
        try
        {
            await ValidateAndThrowAsync(cancellationToken);
            
            return await GetDoctorsAsyncEnumerable(cancellationToken)
                .ToArrayAsync(cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GetDoctorsBySpecializationCommand.ExecuteAsync");
            throw;
        }
    }

    private async IAsyncEnumerable<Doctor> GetDoctorsAsyncEnumerable([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach(var doctorEntity in _context.GetDoctorsBySpecializationAsyncEnumerable(input.Specialization, cancellationToken))
        {
            var specializations = await _context.GetDoctorSpecializationsAsync(doctorEntity, cancellationToken);
            yield return doctorEntity.ToDoctor(specializations);
        }
    }
    
    private async Task ValidateAndThrowAsync(CancellationToken cancellationToken)
    {
        var validator = new InlineValidator<GetDoctorsBySpecializationCommandInput>();

        validator.RuleFor(x => x.Specialization)
            .NotEmpty()
            .WithMessage("Specialization must not be empty.");

        await validator.ValidateAndThrowAsync(input, cancellationToken: cancellationToken);
    }
}

internal class GetDoctorsBySpecializationCommandInput
{
    public required string Specialization { get; init; }
}