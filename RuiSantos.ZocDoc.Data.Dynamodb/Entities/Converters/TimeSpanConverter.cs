using System.Globalization;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Entities.Converters;

public class TimeSpanConverter: IPropertyConverter {
    public DynamoDBEntry ToEntry(object value)
    {
        if (value is not TimeSpan timeSpan)
            throw new ArgumentException($"Invalid TimeSpan: {value}");
        
        return new Primitive(timeSpan.ToString("g"));
    }

    public object FromEntry(DynamoDBEntry entry)
    {
        var value = entry.AsString();
        if (!TimeSpan.TryParseExact(value, "g", CultureInfo.InvariantCulture, out var timeSpan))
            throw new ArgumentException($"Invalid TimeSpan: {value}");
        
        return timeSpan;
    }
}