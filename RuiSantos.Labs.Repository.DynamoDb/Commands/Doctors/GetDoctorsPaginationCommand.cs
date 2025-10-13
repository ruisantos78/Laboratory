using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using FluentValidation;
using Microsoft.Extensions.Logging;
using RuiSantos.Labs.Domain.Commands;
using RuiSantos.Labs.Domain.Data;
using RuiSantos.Labs.Domain.Models;
using RuiSantos.Labs.Repository.DynamoDb.Entities;
using RuiSantos.Labs.Repository.DynamoDb.Entities.Converters;

namespace RuiSantos.Labs.Repository.DynamoDb.Commands.Doctors;

internal class GetDoctorsPaginationCommand(GetDoctorsPaginationCommandInput input
    , IAmazonDynamoDB client
    , ILogger<GetDoctorsPaginationCommand> logger
    ): IAsyncCommand<Pagination<Doctor>>
{
    private const int DefaultPageSize = 25;
    
    private readonly DynamoDBContext _context = new DynamoDBContextBuilder()
        .WithDynamoDBClient(() => client)
        .Build(); 
    
    private string? _paginationToken;
    
    public async Task<Pagination<Doctor>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("GetDoctorsPaginationCommand.ExecuteAsync");
        try
        {
            await ValidateAndThrowAsync(cancellationToken);
            
            var search = _context.GetTargetTable<DoctorEntity>()
                .Scan(new ScanOperationConfig
                {
                    IndexName = DoctorEntity.DoctorLicenceIndexName,
                    Limit = input.Take ?? DefaultPageSize,
                    PaginationToken = _paginationToken
                });

            var entities = await search
                .GetNextSetAsync(cancellationToken)
                .ContinueWith(ConvertToModelAsync, cancellationToken);

            var doctors = await entities.ToListAsync(cancellationToken);
            return new Pagination<Doctor>(doctors, Tokens.Encode(search.PaginationToken));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GetDoctorsPaginationCommand.ExecuteAsync");
            throw;
        }
    }

    private async IAsyncEnumerable<Doctor> ConvertToModelAsync(Task<List<Document>> task)
    {
        foreach (var entity in task.Result.Select(document => _context.FromDocument<DoctorEntity>(document)))
        {
            var specializations = await GetSpecializationsAsync(entity);
            yield return entity.ToDoctor(specializations);
        }
    }
    
    private async Task<string[]> GetSpecializationsAsync(DoctorEntity doctor)
    {
        var query = new QueryOperationConfig
        {
            IndexName = DoctorSpecializationEntity.DoctorSpecializationByDoctorIdIndexName,
            Filter = new QueryFilter(nameof(DoctorSpecializationEntity.DoctorId), QueryOperator.Equal, doctor.Id),
            Select = SelectValues.SpecificAttributes,
            AttributesToGet = [nameof(DoctorSpecializationEntity.Specialization)]
        };

        return await _context.FromQueryAsync<DoctorSpecializationEntity>(query)
            .GetRemainingAsync()
            .ContinueWith(task => task.Result.Select(x => x.Specialization).ToArray());
    }
    
    private async Task ValidateAndThrowAsync(CancellationToken cancellationToken)
    {
        var validator = new InlineValidator<GetDoctorsPaginationCommandInput>();
        
        validator.RuleFor(x => x.Take)
            .GreaterThan(0)
            .When(x => x.Take.HasValue);
        
        validator.RuleFor(x => x.PaginationToken)
            .Custom((token, context) => 
            {
                if (!Tokens.TryDecode(token, out _paginationToken))
                    context.AddFailure("Invalid pagination token format");
            })
            .When(x => !string.IsNullOrWhiteSpace(x.PaginationToken));
        
        await validator.ValidateAndThrowAsync(input, cancellationToken: cancellationToken);
    }
}

internal class GetDoctorsPaginationCommandInput
{
    public int? Take { get; set; }
    public string? PaginationToken { get; set; }
}