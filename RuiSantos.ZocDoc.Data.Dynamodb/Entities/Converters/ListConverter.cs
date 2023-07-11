using System.Text.Json;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Entities.Converters;

public class ListConverter<TModel>: IPropertyConverter
{
    public object FromEntry(DynamoDBEntry entry)
    {
        if (entry is Document document)
        {
            var json = document.ToJson();
            return JsonSerializer.Deserialize<List<TModel>>(json) ?? new List<TModel>();
        }

        return new List<TModel>();
    }

    public DynamoDBEntry ToEntry(object value)
    {
        if (value is List<TModel> model && model.Any())
        {
            var json = JsonSerializer.Serialize(model);
            return (DynamoDBEntry)Document.FromJsonArray(json);
        }

        return new DynamoDBNull();
    }
}

