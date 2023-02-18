using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;

namespace RuiSantos.ZocDoc.Data.Mongodb.Core;

internal sealed class TimeOnlySerializer : StructSerializerBase<TimeOnly>
{
    public static readonly TimeOnlySerializer Instance = new();

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, TimeOnly value)
    {
        context.Writer.WriteString(value.ToString());
    }

    public override TimeOnly Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        return TimeOnly.Parse(context.Reader.ReadString());
    }
}

