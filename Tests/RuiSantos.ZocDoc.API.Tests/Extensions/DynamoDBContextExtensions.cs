using Amazon.DynamoDBv2.DataModel;

namespace RuiSantos.ZocDoc.API.Tests;

internal static class DynamoDBContextExtensions
{
    public static AsyncSearch<TEntity> QueryAsync<TEntity>(this IDynamoDBContext context,
        string indexName, object hashKeyValue)
    {
        return context.QueryAsync<TEntity>(hashKeyValue, new DynamoDBOperationConfig
        {
            IndexName = indexName
        });
    }

    public static async Task<TEntity?> FindAsync<TEntity>(this IDynamoDBContext context,
        string indexName, object hashKeyValue)
    {
        var query = context.QueryAsync<TEntity>(hashKeyValue, new DynamoDBOperationConfig {
            IndexName = indexName
        });

        var entities = await query.GetNextSetAsync();
        return entities.FirstOrDefault();
    }
}

