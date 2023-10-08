using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Newtonsoft.Json;

namespace RuiSantos.Labs.Data.Dynamodb.Entities.Converters;

internal class ListConverter<TModel> : IPropertyConverter
{
    public object FromEntry(DynamoDBEntry entry)
    {
        if (entry is DynamoDBList documentList)
        {
            var models = documentList.AsListOfDocument()
                .Select(x => JsonConvert.DeserializeObject<TModel>(x.ToJson())!);
            return models?.ToList() ?? new List<TModel>();
        }

        return new List<TModel>();
    }

    public DynamoDBEntry ToEntry(object value)
    {
        if (value is List<TModel> model && model.Any())
        {
            var json = JsonConvert.SerializeObject(model);
            var documentList = Document.FromJsonArray(json);
            return new DynamoDBList(documentList.Cast<DynamoDBEntry>());
        }

        return new DynamoDBList();
    }
}

