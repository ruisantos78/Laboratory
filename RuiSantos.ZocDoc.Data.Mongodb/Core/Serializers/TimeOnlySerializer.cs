﻿using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace RuiSantos.ZocDoc.Data.Mongodb.Core.Serializers;

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

