using Amazon.DynamoDBv2.DataModel;

namespace RuiSantos.ZocDoc.API.Tests;

internal static class DynamoDBContextExtensions
{
    public static async Task<IReadOnlyList<TEntity>> FindAll<TEntity>(this IDynamoDBContext context,
        string indexName, object hashKeyValue)
    {
        return await context.QueryAsync<TEntity>(hashKeyValue, new DynamoDBOperationConfig
        {
            IndexName = indexName
        }).GetRemainingAsync();
    }

    public static async Task<TEntity> FindAsync<TEntity>(this IDynamoDBContext context,
        string indexName, object hashKeyValue)
        where TEntity : class, new()
    {
        var query = context.QueryAsync<TEntity>(hashKeyValue, new DynamoDBOperationConfig {
            IndexName = indexName
        });

        var entities = await query.GetNextSetAsync();
        return entities.FirstOrDefault() ?? new();
    }
}

