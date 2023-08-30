using Amazon.DynamoDBv2.DataModel;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Entities.Data;

internal interface IDynamoEntity<TModel> where TModel : class, new() {
    Task LoadFromAsync(IDynamoDBContext context, TModel model);

    Task<TModel> GetModelAsync(IDynamoDBContext context);
}

