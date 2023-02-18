using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Data.Mongodb.Core;
using RuiSantos.ZocDoc.Data.Mongodb.Entities;

namespace RuiSantos.ZocDoc.Data.Mongodb.Mappings;

internal class AppointmentClassMap: IRegisterClassMap
{		
    public void Register()
    {
        BsonClassMap.RegisterClassMap<Appointment>(map =>
        {
            map.AutoMap();
            map.SetDiscriminator(AppointmentEntity.Discriminator);

            map.MapIdMember(e => e.Id).SetIdGenerator(CombGuidGenerator.Instance);
            map.MapMember(e => e.Date).SetSerializer(new DateTimeSerializer(DateTimeKind.Utc));
            map.MapMember(e => e.Patient);
        });
    }
}

