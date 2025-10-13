using Amazon.DynamoDBv2.DataModel;

namespace RuiSantos.Labs.Services.Tests.Fixtures;

internal static class DynamoDbContextExtensions
{
    public static async Task DeleteAllAsync<T>(this DynamoDBContext context) where T : class
    {
        var search = context.ScanAsync<T>(new List<ScanCondition>());
        var items = await search.GetRemainingAsync();
    
        if (items.Count != 0)
        {
            var batchWrite = context.CreateBatchWrite<T>();
            batchWrite.AddDeleteItems(items);
            await batchWrite.ExecuteAsync();
        }
    }
}