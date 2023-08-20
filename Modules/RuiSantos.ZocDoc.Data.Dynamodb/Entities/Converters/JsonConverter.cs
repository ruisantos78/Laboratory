using System.Text.Json;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Entities.Converters;

internal class JsonConverter<TModel>: IPropertyConverter
{
    public object? FromEntry(DynamoDBEntry entry)
    {
        if (entry is Document document)
        {
            var json = document.ToJson();
            return JsonSerializer.Deserialize<TModel>(json);
        }
        
        return default(TModel);
    }

    public DynamoDBEntry ToEntry(object value)
    {
        if (value is TModel model)
        {
            var json = JsonSerializer.Serialize(model);
            return Document.FromJson(json);
        }

        return new DynamoDBNull();
    }
}