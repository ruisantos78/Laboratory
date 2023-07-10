using System.Text.Json;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Entities.Converters;

public class JsonConverter<TModel>: IPropertyConverter {
    public object? FromEntry(DynamoDBEntry entry)
    {
        var json = entry.AsDocument().ToJson();
        return JsonSerializer.Deserialize<TModel>(json);
    }

    public DynamoDBEntry ToEntry(object value)
    {
        var json = JsonSerializer.Serialize(value);
        return Document.FromJson(json);
    }
}