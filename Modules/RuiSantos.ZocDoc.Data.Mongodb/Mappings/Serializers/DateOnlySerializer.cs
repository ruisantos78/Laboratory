using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace RuiSantos.ZocDoc.Data.Mongodb.Mappings.Serializers;

internal sealed class DateOnlySerializer : StructSerializerBase<DateOnly>
{
    private static readonly Lazy<DateOnlySerializer> _instance = new(() => new DateOnlySerializer());

    public static DateOnlySerializer Instance => _instance.Value;

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, DateOnly value)
    {
        context.Writer.WriteString(value.ToString());
    }

    public override DateOnly Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        return DateOnly.Parse(context.Reader.ReadString());
    }
}

