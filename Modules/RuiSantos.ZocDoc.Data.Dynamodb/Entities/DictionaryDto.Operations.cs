using Amazon.DynamoDBv2.DataModel;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Entities;

partial class DictionaryDto
{    
    private static async Task<HashSet<string>> GetAsync(IDynamoDBContext context, string source)
    {
        var items = await context.LoadAsync<DictionaryDto>(source);
        if (items?.Values is null)
            return new();

        return items.Values;
    }

    private static async Task SetAsync(IDynamoDBContext context, string source, IEnumerable<string> values)
    {
        var items = await context.LoadAsync<DictionaryDto>(source);
        if (items?.Values is null)
            items = new DictionaryDto(source);

        values.ToList().ForEach(x => items.Values.Add(x));
        await context.SaveAsync(items);
    }        

    private static async Task RemoveAsync(IDynamoDBContext context, string source, string value)
    {
        var items = await context.LoadAsync<DictionaryDto>(source);
        if (items?.Values.Any() ?? true)
            return;

        items.Values.Remove(value);
        await context.SaveAsync(items);
    }
}