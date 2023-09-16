using Amazon.DynamoDBv2.DataModel;

namespace RuiSantos.Labs.Data.Dynamodb.Entities.Data;

internal static class DynamoOperations
{
    public static async Task<List<TEntity>> FindListAsync<TEntity>(IDynamoDBContext context, List<object> hashKeys) 
    {
        var reader = context.CreateBatchGet<TEntity>();
        hashKeys.ForEach(reader.AddKey);

        await reader.ExecuteAsync();
        return reader.Results.ToList();
    }
}

internal static class DynamoOperations<TModel> where TModel : class, new()
{
    private static async IAsyncEnumerable<TModel> GetModelsAsync<TEntity>(IDynamoDBContext context, IEnumerable<TEntity>? entities)
        where TEntity : IDynamoEntity<TModel>
    {
        if (entities?.Any() is not true)
            yield break;

        var tasks = entities.Select(entity => entity.GetModelAsync(context)).ToList();
        while (tasks.Count > 0)
        {
            var modelTask = await Task.WhenAny(tasks);
            tasks.Remove(modelTask);

            var result = await modelTask;
            if (result is not null)
                yield return result;
        }
    }

    public static async Task<TEntity> StoreAsync<TEntity>(IDynamoDBContext context, TModel model)
        where TEntity : IDynamoEntity<TModel>, new()
    {
        var entity = new TEntity();
        await entity.LoadFromAsync(context, model);

        await context.SaveAsync(entity);
        await DictionaryDto.IncrementCounterAsync(context, entity.TableName);
        
        return entity;
    }

    public static async Task StoreAsync<TEntity>(IDynamoDBContext context, TModel model, params BatchWrite[] writes)
        where TEntity : IDynamoEntity<TModel>, new()
    {
        var entity = new TEntity();
        await entity.LoadFromAsync(context, model);

        var writer = context.CreateBatchWrite<TEntity>();
        writer.AddPutItem(entity);
        await DictionaryDto.IncrementCounterAsync(context, entity.TableName);

        var batch = writes.Append(writer).ToArray();        
        await context.ExecuteBatchWriteAsync(batch.ToArray());
    }

    public static async Task<TModel?> FindAsync<TEntity>(IDynamoDBContext context, object hashKey)
        where TEntity : IDynamoEntity<TModel>
    {
        var entity = await context.LoadAsync<TEntity>(hashKey);
        if (entity is null) 
            return default;

        return await entity.GetModelAsync(context);
    }

    public static async Task<List<TModel>> FindListAsync<TEntity>(IDynamoDBContext context, List<object> hashKeys)
        where TEntity : IDynamoEntity<TModel>
    {
        var reader = context.CreateBatchGet<TEntity>();
        hashKeys.ForEach(reader.AddKey);

        await reader.ExecuteAsync();

        return await GetModelsAsync(context, reader.Results).ToListAsync();
    }

    public static async Task<List<TModel>> FindAllAsync<TEntity>(IDynamoDBContext context, string indexName, object hashKey)
        where TEntity : IDynamoEntity<TModel>
    {
        var config = new DynamoDBOperationConfig
        {
            IndexName = indexName
        };

        var entities = await context.QueryAsync<TEntity>(hashKey, config).GetRemainingAsync();

        return await GetModelsAsync(context, entities).ToListAsync();
    }
}