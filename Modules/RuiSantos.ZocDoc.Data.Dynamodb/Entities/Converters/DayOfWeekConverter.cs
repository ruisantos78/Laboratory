using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;

namespace RuiSantos.ZocDoc.Data.Dynamodb;

internal class DayOfWeekConverter : IPropertyConverter
{
    public object FromEntry(DynamoDBEntry entry)
    {
        if (entry is not Primitive primitive)
            return default(DayOfWeek);

        var value = primitive.AsString();
        if (!Enum.TryParse(value, out DayOfWeek dayOfWeek))
            return default(DayOfWeek);

        return dayOfWeek;
    }   

    public DynamoDBEntry ToEntry(object value)
    {
        if (value is not DayOfWeek dayOfWeek)
            return new DynamoDBNull();
        
        return new Primitive(dayOfWeek.ToString());
    }
}