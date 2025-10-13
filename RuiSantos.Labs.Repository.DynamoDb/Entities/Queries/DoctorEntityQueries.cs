using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;

namespace RuiSantos.Labs.Repository.DynamoDb.Entities.Queries;

internal static class DoctorEntityQueries
{
    public static async Task<Guid> GetDoctorIdByLicenseAsync(this DynamoDBContext context, string license
        , CancellationToken cancellationToken = default)
    {
        var query = new QueryOperationConfig
        {
            IndexName = DoctorEntity.DoctorLicenceIndexName,
            Filter = new QueryFilter(nameof(DoctorEntity.License), QueryOperator.Equal, license),
            Limit = 1,
            Select = SelectValues.SpecificAttributes,
            AttributesToGet = [nameof(DoctorEntity.Id)]
        };

        var entity = await context.FromQueryAsync<DoctorEntity>(query)
            .GetNextSetAsync(cancellationToken);

        return entity.FirstOrDefault()?.Id ?? Guid.Empty;
    }
    
    public static async Task<DoctorEntity?> GetDoctorByLicenseAsync(this DynamoDBContext context, string license
        , CancellationToken cancellationToken = default)
    {
        var query = new QueryOperationConfig
        {
            IndexName = DoctorEntity.DoctorLicenceIndexName,
            Filter = new QueryFilter(nameof(DoctorEntity.License), QueryOperator.Equal, license),
            Limit = 1
        };

        var entity = await context.FromQueryAsync<DoctorEntity>(query)
            .GetNextSetAsync(cancellationToken);

        return entity.FirstOrDefault();
    }
}