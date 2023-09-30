using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RuiSantos.Labs.Core.Models.Converters;

public class TimeSpanConverter : JsonConverter
{
    /// <summary>
    /// Writes the JSON representation of the object. This is used to ensure that time spans are in sync with the data being written
    /// </summary>
    /// <param name="writer">The to write to.</param>
    /// <param name="value">The object to write. Must be an IEnumerable of TimeSpan objects otherwise an exception is thrown.</param>
    /// <param name="serializer">The to use when serializing to JSON. If null the default serializer is used</param>
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value is not IEnumerable<TimeSpan> timespans)
            throw new JsonSerializationException("Expected an IEnumerable<TimeSpan> but got something else");
        
        var jsonArray = new JArray(timespans.Select(x => x.ToString("g")));
        jsonArray.WriteTo(writer);
    }

    /// <summary>
    /// Reads the JSON representation of the object. This method is called when the method returns null. The reader is positioned on the property with the name of the property being read.
    /// </summary>
    /// <param name="reader">The to read from.</param>
    /// <param name="objectType">Type of the object. Only public for compatibility.</param>
    /// <param name="existingValue">The existing value of property being read.</param>
    /// <param name="serializer">The serializer to use for deserialization. Only public for compatibility.</param>
    /// <returns>The time span ( s ) read from the reader. The list can be empty but never null. If null is returned the reader is positioned on the property with the name of the property</returns>
    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        return JArray.Load(reader)
            .Where(token => token.Type == JTokenType.String)
            .Select(token => TimeSpan.Parse(token.ToString()))
            .ToList();
    }

    /// <summary>
    /// Determines whether this instance can convert the specified object type. This is determined by the type being an IEnumerable
    /// </summary>
    /// <param name="objectType">Type of the object to convert.</param>
    /// <returns>True if this instance can convert the specified object type ; otherwise false. If false is returned the conversion will fail</returns>
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(IEnumerable<TimeSpan>);
    }
}