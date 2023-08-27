using Amazon.DynamoDBv2.DataModel;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Entities;

partial class DictionaryDto
{    
    private static async Task<IReadOnlyDictionary<Guid, string>> GetAsync(IDynamoDBContext context, string source)
    {
        var items = await context.LoadAsync<DictionaryDto>(source);
        if (items?.Values is null)
            return new Dictionary<Guid, string>();

        return items.Values;
    }

    private static async Task SetAsync(IDynamoDBContext context, string source, string value)
    {
        var items = await context.LoadAsync<DictionaryDto>(source);                
        if (items?.Values.ContainsValue(value) is true)
        {
            items.Values.Add(Guid.NewGuid(), value);
            await context.SaveAsync(items);
        }
    }

    private static async Task RemoveAsync(IDynamoDBContext context, string source, string value)
    {
        var items = await context.LoadAsync<DictionaryDto>(source);
        if (items?.Values.Any() is not true)
            return;

        var removeItems = items.Values.Where(x => x.Value == value).ToList();
        if (!removeItems.Any())        
            return;
        
        removeItems.ForEach(x => items.Values.Remove(x.Key));
        await context.SaveAsync(items);
    }
}