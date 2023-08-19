using System.Globalization;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Entities.Converters;

public class DateTimeConverter: IPropertyConverter {
    public DynamoDBEntry ToEntry(object value)
    {
        if (value is not DateTime dateTime)
            return new DynamoDBNull();
        
        return new Primitive(dateTime.ToUniversalTime().ToString("u"));
    }

    public object FromEntry(DynamoDBEntry entry)
    {
        if (entry is not Primitive)
            return default(DateTime);    
            
        var value = entry.AsString();
        if (!DateTime.TryParseExact(value, "u", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var dateTime))
            return default(DateTime);
        
        return dateTime;
    }
}