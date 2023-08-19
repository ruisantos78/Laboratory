using Amazon.DynamoDBv2.DataModel;

namespace RuiSantos.ZocDoc.Data.Dynamodb;

internal abstract class DynamoDataObject<TEntity> where TEntity : class, new()
{    
    private static async IAsyncEnumerable<TEntity> ToEntitiesAsync<TDto>(IDynamoDBContext context, IEnumerable<TDto>? dtos)
        where TDto : DynamoDataObject<TEntity>
    {
        if (dtos?.Any() is not true) 
           yield break;
           
        var tasks = dtos.Select(dto => dto.ToEntityAsync(context))
            .ToList();
            
        while (tasks.Count > 0) {
            var completedTask = await Task.WhenAny(tasks);
            tasks.Remove(completedTask);

            var result = await completedTask;
            if (result is not null) {
                yield return result;
            }
        }
    }

    protected abstract Task<TEntity> ToEntityAsync(IDynamoDBContext context);

    protected abstract Task FromEntityAsync(IDynamoDBContext context, TEntity entity);

    protected static async Task StoreAsync<TDto>(IDynamoDBContext context, TEntity entity)
        where TDto : DynamoDataObject<TEntity>, new()
    {
        var dto = new TDto();
        await dto.FromEntityAsync(context, entity);
        await context.SaveAsync(dto);
    }

    protected static async Task StoreAsync<TDto>(IDynamoDBContext context, TEntity entity, params BatchWrite[] writes) 
        where TDto : DynamoDataObject<TEntity>, new()
    {
        var dto = new TDto();
        await dto.FromEntityAsync(context, entity);

        var dtoWriter = context.CreateBatchWrite<TDto>();
        dtoWriter.AddPutItem(dto);     

        var batchList = new List<BatchWrite>() { dtoWriter };
        batchList.AddRange(writes);

        await context.ExecuteBatchWriteAsync(batchList.ToArray());
    }   

    protected static async Task<TEntity?> FindAsync<TDto>(IDynamoDBContext context, object id)
        where TDto : DynamoDataObject<TEntity>
    {
        var dto = await context.LoadAsync<TDto>(id);
        return dto is null ? null : await dto.ToEntityAsync(context);    
    }

    protected static async Task<List<TEntity>> SearchAsync<TDto>(IDynamoDBContext context, string indexName, object id)
        where TDto : DynamoDataObject<TEntity>
    {
        var config = new DynamoDBOperationConfig
        {
            IndexName = indexName            
        };

        var dtos = await context.QueryAsync<TDto>(id, config).GetRemainingAsync();
        
        return await ToEntitiesAsync(context, dtos).ToListAsync();        
    }

    protected static async Task<List<TEntity>> FindListAsync<TDto>(IDynamoDBContext context, List<object> ids)
        where TDto : DynamoDataObject<TEntity>
    {
        var reader = context.CreateBatchGet<TDto>();
        ids.ForEach(reader.AddKey);
        await reader.ExecuteAsync();

        return await ToEntitiesAsync(context, reader.Results).ToListAsync(); 
    }
}