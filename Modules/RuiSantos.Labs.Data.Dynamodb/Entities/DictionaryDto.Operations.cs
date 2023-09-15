using Amazon.DynamoDBv2.DataModel;

namespace RuiSantos.Labs.Data.Dynamodb.Entities;

partial class DictionaryDto
{    
    private static async Task<HashSet<string>> GetAsync(IDynamoDBContext context, string source)
    {
        var entities = await context.QueryAsync<DictionaryDto>(source).GetRemainingAsync();
        return entities?.Select(s => s.Value).ToHashSet() ?? new();
    }

    private static async Task SetAsync(IDynamoDBContext context, string source, IEnumerable<string> values)
    {
        if (!values.Any())
            return;

        var writer = context.CreateBatchWrite<DictionaryDto>();
        writer.AddPutItems(values.Select(value => new DictionaryDto { Source = source, Value = value }));
        await writer.ExecuteAsync();        
    }        

    private static async Task RemoveAsync(IDynamoDBContext context, string source, string value)
    {
        await context.DeleteAsync<DictionaryDto>(hashKey: source, rangeKey: value);        
    }
}