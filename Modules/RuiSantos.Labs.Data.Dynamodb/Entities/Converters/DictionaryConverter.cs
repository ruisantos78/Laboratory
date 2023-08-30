using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using System.Text.Json;

namespace RuiSantos.Labs.Data.Dynamodb.Entities.Converters;

internal class DictionaryConverter<TKey, TValue> : IPropertyConverter
    where TKey : struct
{
    public object FromEntry(DynamoDBEntry entry)
    {
        if (entry is Document document)
        {
            var json = document.ToJson();
            return JsonSerializer.Deserialize<Dictionary<TKey, TValue>>(json) ?? new Dictionary<TKey, TValue>();
        }

        return new Dictionary<TKey, TValue>();
    }

    public DynamoDBEntry ToEntry(object value)
    {
        if (value is Dictionary<TKey, TValue> model && model.Any())
        {
            var json = JsonSerializer.Serialize(model);
            return Document.FromJson(json);
        }

        return new DynamoDBNull();
    }
}


