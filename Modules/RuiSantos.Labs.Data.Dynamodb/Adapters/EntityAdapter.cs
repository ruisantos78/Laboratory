using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

namespace RuiSantos.Labs.Data.Dynamodb.Adapters;

internal static class EntityAdapterExtensions 
{
    public static async IAsyncEnumerable<TModel> ToModelsAsync<TModel>(this Task<IEnumerable<Task<TModel>>> tasks) 
    {
        if (tasks is null)
            yield break;
    
        var operations = (await tasks.ContinueWith(x => x.Result)).ToHashSet();
        while (operations.Count > 0)
        {
            var operation = await Task.WhenAny(operations);
            operations.Remove(operation);

            var result = await operation;
            if (result is not null)
                yield return result;
        }
    } 

    public static async IAsyncEnumerable<TModel> ToModelsAsync<TModel>(this IEnumerable<Task<TModel>> tasks) 
    {   
        var operations = tasks.ToHashSet();
        while (operations.Count > 0)
        {
            var operation = await Task.WhenAny(operations);
            operations.Remove(operation);

            var result = await operation;
            if (result is not null)
                yield return result;
        }
    }     
}

internal abstract class EntityAdapter<TEntity, TModel>
{
    protected IDynamoDBContext Context { get; }

    protected EntityAdapter(IAmazonDynamoDB client)
    {
        this.Context = new DynamoDBContext(client);
    } 

    protected abstract Task<TEntity> ToEntityAsync(TModel model);

    protected abstract Task<TModel> ToModelAsync(TEntity entity);   
}
