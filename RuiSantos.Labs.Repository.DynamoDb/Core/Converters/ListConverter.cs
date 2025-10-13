using System.Text.Json;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;

namespace RuiSantos.Labs.Repository.DynamoDb.Core.Converters;

internal class ListConverter<TModel> : IPropertyConverter
{
    public object FromEntry(DynamoDBEntry entry)
    {
        if (entry is DynamoDBList documentList)
        {
            var models = documentList.AsListOfDocument()
                .Select(x => JsonSerializer.Deserialize<TModel>(x.ToJson())!);
            return models?.ToList() ?? new List<TModel>();
        }

        return new List<TModel>();
    }

    public DynamoDBEntry ToEntry(object value)
    {
        if (value is List<TModel> model && model.Any())
        {
            var json = JsonSerializer.Serialize(model);
            var documentList = Document.FromJsonArray(json);
            return new DynamoDBList(documentList.Cast<DynamoDBEntry>());
        }

        return new DynamoDBList();
    }
}

