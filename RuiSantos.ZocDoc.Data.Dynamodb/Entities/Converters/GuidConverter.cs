using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Entities.Converters;

public class GuidConverter: IPropertyConverter {
    public DynamoDBEntry ToEntry(object value)
    {
        if (value is not Guid guid)
            throw new ArgumentException($"Invalid Guid: {value}");
        
        return new Primitive(guid.ToString("D"));
    }

    public object FromEntry(DynamoDBEntry entry)
    {
        var value = entry.AsString();
        if (!Guid.TryParseExact(value, "D", out var guid))
            throw new ArgumentException($"Invalid Guid: {value}");
        
        return guid;
    }
}