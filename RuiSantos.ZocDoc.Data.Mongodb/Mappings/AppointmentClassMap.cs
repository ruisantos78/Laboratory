using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using RuiSantos.ZocDoc.Data.Mongodb.Core;
using RuiSantos.ZocDoc.Data.Mongodb.Entities;

namespace RuiSantos.ZocDoc.Data.Mongodb.Mappings;

internal class AppointmentClassMap: IRegisterClassMap
{		
    public void Register()
    {
        BsonClassMap.RegisterClassMap<AppointmentEntity>(map =>
        {
            map.AutoMap();
            map.SetDiscriminator(AppointmentEntity.Discriminator);

            map.MapIdMember(e => e.Id).SetIdGenerator(StringObjectIdGenerator.Instance);
            map.MapProperty(e => e.PatientReference);

            map.MapMember(e => e.Date).SetSerializer(new DateTimeSerializer(DateTimeKind.Utc));
            map.UnmapMember(e => e.Patient);
        });
    }
}

