using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace RuiSantos.ZocDoc.Data.Mongodb.Core.Serializers;

internal sealed class DateOnlySerializer : StructSerializerBase<DateOnly>
{
    public static readonly DateOnlySerializer Instance = new();

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, DateOnly value)
    {
        context.Writer.WriteString(value.ToString());
    }

    public override DateOnly Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        return DateOnly.Parse(context.Reader.ReadString());
    }
}

