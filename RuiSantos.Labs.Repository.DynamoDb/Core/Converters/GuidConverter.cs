using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;

namespace RuiSantos.Labs.Repository.DynamoDb.Core.Converters;

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
            return Guid.Empty;

        var value = primitive.AsString();
        if (!Guid.TryParse(value, out var guid))
            return Guid.Empty;

        return guid;
    }
}