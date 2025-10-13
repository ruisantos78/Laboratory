using Amazon.DynamoDBv2.DataModel;

namespace RuiSantos.Labs.Services.Tests.Fixtures;

internal static class DynamoDbContextExtensions
{
    public static async Task DeleteAllAsync<T>(this DynamoDBContext context) where T : class
    {
        var batchWrite = context.CreateBatchWrite<T>();
        
        var search = context.ScanAsync<T>([]);
        while (!search.IsDone)
        {
            var items = await search.GetNextSetAsync();
            if (items.Count != 0)
                batchWrite.AddDeleteItems(items);
        }
        
        await batchWrite.ExecuteAsync();
    }
}