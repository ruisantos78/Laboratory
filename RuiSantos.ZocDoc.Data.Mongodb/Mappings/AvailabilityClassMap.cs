using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using RuiSantos.ZocDoc.Data.Mongodb.Core;
using RuiSantos.ZocDoc.Core.Models;

namespace RuiSantos.ZocDoc.Data.Mongodb.Mappings;

internal class AvailabilityClassMap: IRegisterClassMap
{
    public void Register()
    {
        BsonClassMap.RegisterClassMap<Availability>(map =>
        {
            map.AutoMap();

            map.MapMember(e => e.DayOfWeek).SetSerializer(new EnumSerializer<DayOfWeek>(BsonType.String));
            map.MapMember(e => e.StartTime).SetSerializer(new StringSerializer());
            map.MapMember(e => e.Duration);
        });
    }
}

