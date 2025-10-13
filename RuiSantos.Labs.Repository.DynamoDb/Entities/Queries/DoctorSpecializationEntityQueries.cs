using System.Runtime.CompilerServices;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;

namespace RuiSantos.Labs.Repository.DynamoDb.Entities.Queries;

internal static class DoctorSpecializationEntityQueries
{
    public static async Task<List<string>> GetDoctorSpecializationsAsync(this DynamoDBContext context
        , DoctorEntity doctorEntity
        , CancellationToken cancellationToken = default)
    {
        var query = await context
            .QueryAsync<DoctorSpecializationEntity>(doctorEntity.Id)
            .GetRemainingAsync(cancellationToken);

        return [..query.Select(x => x.Description)];
    }

    public static async IAsyncEnumerable<DoctorSpecializationEntity> GetDoctorSpecializationAsyncEnumerable(
        this DynamoDBContext context
        , DoctorEntity doctorEntity
        , [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var query = new QueryOperationConfig
        {
            Filter = new QueryFilter(nameof(DoctorSpecializationEntity.DoctorId), QueryOperator.Equal, doctorEntity.Id),
            Limit = 50,
        };
        
        var search = context.FromQueryAsync<DoctorSpecializationEntity>(query);
        while (!search.IsDone)
        {   
            var resultset = await search.GetNextSetAsync(cancellationToken);
            foreach (var entity in resultset)
            {
                yield return entity;
            }
        }
    }
    
    public static async IAsyncEnumerable<DoctorEntity> GetDoctorsBySpecializationAsyncEnumerable(this DynamoDBContext context
        , string specialization
        , [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var query = new QueryOperationConfig
        {
            IndexName = DoctorSpecializationEntity.DoctorSpecializationIndexName,
            Filter = new QueryFilter(nameof(DoctorSpecializationEntity.Specialization), QueryOperator.Equal, specialization.ToLower()),
            Select = SelectValues.SpecificAttributes,
            Limit = 50,
            AttributesToGet = [nameof(DoctorSpecializationEntity.DoctorId)]
        };

        var search = context.FromQueryAsync<DoctorSpecializationEntity>(query);
        while (!search.IsDone)
        {   
            var resultset = await search.GetNextSetAsync(cancellationToken);
            foreach (var item in resultset)
            {
                var doctorEntity = await context.LoadAsync<DoctorEntity>(item.DoctorId, cancellationToken);
                if (doctorEntity is not null)
                    yield return doctorEntity;
            }
        }
    }
}