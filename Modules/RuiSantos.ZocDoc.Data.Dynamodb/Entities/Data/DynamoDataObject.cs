using Amazon.DynamoDBv2.DataModel;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Entities.Data;

/// <summary>
/// Base class for managing interaction between DynamoDB entities and their DTOs (Data Transfer Objects).
/// </summary>
/// <typeparam name="TEntity">The entity type to work with.</typeparam>
internal abstract class DynamoDataObject<TEntity> where TEntity : class, new()
{
    /// <summary>
    /// Converts a collection of DTOs to entity instances asynchronously.
    /// </summary>
    /// <typeparam name="TDto">The DTO type to convert from.</typeparam>
    /// <param name="context">The DynamoDB context to use.</param>
    /// <param name="dtos">Collection of DTOs to convert.</param>
    /// <returns>An asynchronous enumerable of entity instances.</returns>
    private static async IAsyncEnumerable<TEntity> ToEntitiesAsync<TDto>(IDynamoDBContext context, IEnumerable<TDto>? dtos)
        where TDto : DynamoDataObject<TEntity>
    {
        if (dtos?.Any() is not true)
            yield break;

        var tasks = dtos.Select(dto => dto.ToEntityAsync(context))
            .ToList();

        while (tasks.Count > 0)
        {
            var completedTask = await Task.WhenAny(tasks);
            tasks.Remove(completedTask);

            var result = await completedTask;
            if (result is not null)
            {
                yield return result;
            }
        }
    }

    /// <summary>
    /// Converts the DTO to an entity asynchronously.
    /// </summary>
    /// <param name="context">The DynamoDB context to use.</param>
    /// <returns>An asynchronous task representing the entity conversion.</returns>
    protected abstract Task<TEntity> ToEntityAsync(IDynamoDBContext context);

    /// <summary>
    /// Converts the entity to the DTO asynchronously.
    /// </summary>
    /// <param name="context">The DynamoDB context to use.</param>
    /// <param name="entity">The entity instance to convert.</param>
    /// <returns>An asynchronous task representing the DTO conversion.</returns>
    protected abstract Task FromEntityAsync(IDynamoDBContext context, TEntity entity);

    protected static async Task<TDto> StoreAsync<TDto>(IDynamoDBContext context, TEntity entity)
        where TDto : DynamoDataObject<TEntity>, new()
    {
        var dto = new TDto();
        await dto.FromEntityAsync(context, entity);
        await context.SaveAsync(dto);

        return dto;
    }

    /// <summary>
    /// Stores an entity in DynamoDB using its corresponding DTO asynchronously.
    /// </summary>
    /// <typeparam name="TDto">The DTO type to use.</typeparam>
    /// <param name="context">The DynamoDB context to use.</param>
    /// <param name="entity">The entity to store.</param>
    /// <returns>An asynchronous task representing the storage operation.</returns>
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

    /// <summary>
    /// Finds a single entity in DynamoDB using the specified ID asynchronously.
    /// </summary>
    /// <typeparam name="TDto">The DTO type to use.</typeparam>
    /// <param name="context">The DynamoDB context to use.</param>
    /// <param name="id">The ID of the entity to find.</param>
    /// <returns>An asynchronous task representing the entity retrieval operation. Returns null if not found.</returns>
    protected static async Task<TEntity?> FindAsync<TDto>(IDynamoDBContext context, object id)
        where TDto : DynamoDataObject<TEntity>
    {
        var dto = await context.LoadAsync<TDto>(id);
        return dto is null ? null : await dto.ToEntityAsync(context);
    }

    /// <summary>
    /// Finds a list of entities in DynamoDB using a list of IDs asynchronously.
    /// </summary>
    /// <typeparam name="TDto">The DTO type to use.</typeparam>
    /// <param name="context">The DynamoDB context to use.</param>
    /// <param name="ids">The list of IDs of entities to find.</param>
    /// <returns>An asynchronous task representing the entity list retrieval operation.</returns>
    protected static async Task<List<TEntity>> FindListAsync<TDto>(IDynamoDBContext context, List<object> ids)
        where TDto : DynamoDataObject<TEntity>
    {
        var reader = context.CreateBatchGet<TDto>();
        ids.ForEach(reader.AddKey);
        await reader.ExecuteAsync();

        return await ToEntitiesAsync(context, reader.Results).ToListAsync();
    }

    /// <summary>
    /// Searches for entities in DynamoDB using a specified index and ID asynchronously.
    /// </summary>
    /// <typeparam name="TDto">The DTO type to use.</typeparam>
    /// <param name="context">The DynamoDB context to use.</param>
    /// <param name="indexName">The name of the index to query.</param>
    /// <param name="id">The ID to search for.</param>
    /// <returns>An asynchronous task representing the search operation.</returns>
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
}