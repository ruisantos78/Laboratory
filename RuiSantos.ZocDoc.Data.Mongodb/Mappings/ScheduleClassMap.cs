using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using RuiSantos.ZocDoc.Data.Mongodb.Core;
using RuiSantos.ZocDoc.Data.Mongodb.Entities;

namespace RuiSantos.ZocDoc.Data.Mongodb.Mappings;

internal class ScheduleClassMap : IRegisterClassMap
{
    public void Register()
    {
        BsonClassMap.RegisterClassMap<ScheduleEntity>(map =>
        {
            map.AutoMap();
            map.SetDiscriminator(ScheduleEntity.Discriminator);

            map.MapIdMember(e => e.Id).SetIdGenerator(StringObjectIdGenerator.Instance);

            map.MapProperty(e => e.DoctorReference);
            map.MapProperty(e => e.AppointmentsReferences);

            map.MapMember(e => e.Availability);

            map.UnmapMember(e => e.Doctor);
            map.UnmapMember(e => e.Appointments);
        });
    }
}

