using Amazon.DynamoDBv2.DataModel;

namespace RuiSantos.Labs.Data.Dynamodb.Entities;

partial class DictionaryDto
{	
    public static async Task<long> GetCounterAsync(IDynamoDBContext context, string tableName)
    {
        var counter = (await GetAsync(context, $"{tableName}_counter"))
			.FirstOrDefault();

		return long.TryParse(counter, out var value) ? value : 0;
    }

	public static async Task IncrementCounterAsync(IDynamoDBContext context, string tableName)
    {
        var counter = await GetCounterAsync(context, tableName);
		await SetAsync(context, $"{tableName}_counter", counter + 1);
	}

	public static async Task DecrementCounterAsync(IDynamoDBContext context, string tableName)
    {
        var counter = await GetCounterAsync(context, tableName);
		if (counter == 0)
			return;

		await SetAsync(context, $"{tableName}_counter", counter - 1);
	}
}