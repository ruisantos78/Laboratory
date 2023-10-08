using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;

namespace RuiSantos.Labs.Data.Dynamodb.Entities.Converters;

internal class GuidConverter : IPropertyConverter
{
    public DynamoDBEntry ToEntry(object value)
    {
        if (value is not Guid guid)
            return new DynamoDBNull();

        return new Primitive(guid.ToString());
    }

    public object FromEntry(DynamoDBEntry entry)
    {
        if (entry is not Primitive primitive)
            return default(Guid);

        var value = primitive.AsString();
        if (!Guid.TryParse(value, out var guid))
            return default(Guid);

        return guid;
    }
}