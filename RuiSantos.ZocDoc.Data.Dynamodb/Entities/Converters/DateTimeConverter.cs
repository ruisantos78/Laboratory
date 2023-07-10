using System.Globalization;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Entities.Converters;

public class DateTimeConverter: IPropertyConverter {
    public DynamoDBEntry ToEntry(object value)
    {
        if (value is not DateTime dateTime)
            throw new ArgumentException($"Invalid DateTime: {value}");
        
        return new Primitive(dateTime.ToUniversalTime().ToString("u"));
    }

    public object FromEntry(DynamoDBEntry entry)
    {
        var value = entry.AsString();
        if (!DateTime.TryParseExact(value, "u", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var dateTime))
            throw new ArgumentException($"Invalid DateTime: {value}");
        
        return dateTime;
    }
}