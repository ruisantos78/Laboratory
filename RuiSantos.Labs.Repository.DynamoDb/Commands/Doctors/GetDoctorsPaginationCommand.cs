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

internal class GetDoctorsPaginationCommand(GetDoctorsPaginationCommandInput input
    , IAmazonDynamoDB client
    , ILogger<GetDoctorsPaginationCommand> logger
    ): IAsyncCommand<Pagination<Doctor>>
{    
    private readonly DynamoDBContext _context = new DynamoDBContextBuilder()
        .WithDynamoDBClient(() => client)
        .Build(); 
       
    public async Task<Pagination<Doctor>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("GetDoctorsPaginationCommand.ExecuteAsync");
        try
        {
            await ValidateAndThrowAsync(cancellationToken);

            var search = _context.FromScanAsync<DoctorEntity>(new ScanOperationConfig
            {
                IndexName = DoctorEntity.DoctorLicenceIndexName,
                Limit = input.Take ?? int.MaxValue,
                PaginationToken = input.PaginationToken
            });            

            var entities = await search
                .GetNextSetAsync(cancellationToken)
                .ContinueWith(ConvertToModelAsync, cancellationToken);

            var doctors = await entities.ToListAsync(cancellationToken);
            return new Pagination<Doctor>(doctors, search.PaginationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GetDoctorsPaginationCommand.ExecuteAsync");
            throw;
        }
    }

    private async IAsyncEnumerable<Doctor> ConvertToModelAsync(Task<List<DoctorEntity>> task)
    {
        foreach (var doctorEntity in task.Result)
        {
            var specializations = await _context.GetDoctorSpecializationsAsync(doctorEntity);            
            yield return doctorEntity.ToDoctor(specializations);
        }
    }
    
    private async Task ValidateAndThrowAsync(CancellationToken cancellationToken)
    {
        var validator = new InlineValidator<GetDoctorsPaginationCommandInput>();
        
        validator.RuleFor(x => x.Take)
            .GreaterThan(0)
            .When(x => x.Take.HasValue);
        
        validator.RuleFor(x => x.PaginationToken)
            .Matches(@"^\{\s*""License""\s*:\s*\{\s*""S""\s*:\s*"".*?""\s*\},\s*""Id""\s*:\s*\{\s*""S""\s*:\s*""[0-9a-fA-F\-]{36}""\s*\}\s*\}$")
            .When(x => !string.IsNullOrWhiteSpace(x.PaginationToken))
            .WithMessage("Invalid DynamoDB pagination token format.");
        
        await validator.ValidateAndThrowAsync(input, cancellationToken: cancellationToken);
    }
}

internal struct GetDoctorsPaginationCommandInput
{
    public int? Take { get; init; }
    public string? PaginationToken { get; init; }
}