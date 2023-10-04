using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

namespace RuiSantos.Labs.Data.Dynamodb.Adapters;

internal abstract class EntityModelAdapter<TEntity, TModel>
{
    protected IDynamoDBContext Context { get; }

    protected EntityModelAdapter(IAmazonDynamoDB client)
    {
        this.Context = new DynamoDBContext(client);
    } 

    protected abstract Task<TEntity> AsEntityAsync(TModel model);

    protected abstract Task<TModel> AsModelAsync(TEntity entity);   
}