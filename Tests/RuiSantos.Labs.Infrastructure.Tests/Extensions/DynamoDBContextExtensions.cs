using Amazon.DynamoDBv2.DataModel;

namespace RuiSantos.Labs.Infrastructure.Tests.Extensions;

internal static class DynamoDbContextExtensions
{
    public static async Task<IReadOnlyList<TEntity>> FindAllAsync<TEntity>(this IDynamoDBContext context,
        object hashKeyValue, string? indexName = null)
    {
        if (string.IsNullOrWhiteSpace(indexName))
            return await context.QueryAsync<TEntity>(hashKeyValue).GetRemainingAsync();

        return await context.QueryAsync<TEntity>(hashKeyValue, new QueryConfig
        {
            IndexName = indexName
        }).GetRemainingAsync();
    }

    public static async Task<TEntity> FindAsync<TEntity>(this IDynamoDBContext context,
        object hashKeyValue, string? indexName = null)
        where TEntity : class, new()
    {
        if (string.IsNullOrWhiteSpace(indexName))
            return await context.LoadAsync<TEntity>(hashKeyValue);

        var query = context.QueryAsync<TEntity>(hashKeyValue, new QueryConfig
        {
            IndexName = indexName
        });

        var entities = await query.GetNextSetAsync();
        return entities.FirstOrDefault() ?? new();
    }
}

