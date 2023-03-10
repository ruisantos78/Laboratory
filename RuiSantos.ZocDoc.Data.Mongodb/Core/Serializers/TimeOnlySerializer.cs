using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace RuiSantos.ZocDoc.Data.Mongodb.Core.Serializers;

internal sealed class TimeOnlySerializer : StructSerializerBase<TimeOnly>
{
    private static readonly Lazy<TimeOnlySerializer> _instance = new(() => new TimeOnlySerializer());

    public static TimeOnlySerializer Instance => _instance.Value;

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, TimeOnly value)
    {
        context.Writer.WriteString(value.ToString());
    }

    public override TimeOnly Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        return TimeOnly.Parse(context.Reader.ReadString());
    }
}

