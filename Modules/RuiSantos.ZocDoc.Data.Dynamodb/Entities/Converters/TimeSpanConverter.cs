using System.Globalization;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Entities.Converters;

public class TimeSpanConverter: IPropertyConverter {
    public DynamoDBEntry ToEntry(object value)
    {
        if (value is not TimeSpan timeSpan)
            return new DynamoDBNull();
        
        return new Primitive(timeSpan.ToString("g"));
    }

    public object FromEntry(DynamoDBEntry entry)
    {
        if (entry is not Primitive primitive)
            return default(TimeSpan);
        
        var value = primitive.AsString();
        if (!TimeSpan.TryParseExact(value, "g", CultureInfo.InvariantCulture, out var timeSpan))
            return default(TimeSpan);
        
        return timeSpan;
    }
}