using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RuiSantos.ZocDoc.Core;

public class TimeSpanConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value is not IEnumerable<TimeSpan> timespans)
            throw new JsonSerializationException("Expected an IEnumerable<TimeSpan> but got something else");
        
        var formattedTimeSpans = timespans.Select(ts => ts.ToString("g"));        
        var formattedArray = new JArray(formattedTimeSpans);
        formattedArray.WriteTo(writer);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        var timespans = JArray.Load(reader).Select(token =>
        {
            if (token.Type == JTokenType.String && TimeSpan.TryParse(token.ToString(), out var timeSpan))
                return timeSpan;

            throw new JsonSerializationException("Invalid TimeSpan format");
        });
        
        return timespans.ToList();
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(IEnumerable<TimeSpan>);
    }
}