using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;

namespace RuiSantos.ZocDoc.Data.Dynamodb;

public class DayOfWeekConverter : IPropertyConverter
{
    public object FromEntry(DynamoDBEntry entry)
    {
        var value = entry.AsString();
        if (!Enum.TryParse(value, out DayOfWeek dayOfWeek))
            throw new ArgumentException($"Invalid DayOfWeek: {value}");

        return dayOfWeek;
    }   

    public DynamoDBEntry ToEntry(object value)
    {
        if (value is not DayOfWeek dayOfWeek)
            throw new ArgumentException($"Invalid DayOfWeek: {value}");
            
        return dayOfWeek.ToString();
    }
}