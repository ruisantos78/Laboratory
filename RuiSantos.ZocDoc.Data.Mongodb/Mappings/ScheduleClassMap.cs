using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Data.Mongodb.Core;
using RuiSantos.ZocDoc.Data.Mongodb.Entities;

namespace RuiSantos.ZocDoc.Data.Mongodb.Mappings;

internal class ScheduleClassMap : IRegisterClassMap
{
    public void Register()
    {
        BsonClassMap.RegisterClassMap<Schedule>(map =>
        {
            map.AutoMap();
            map.SetDiscriminator(ScheduleEntity.Discriminator);

            map.MapIdMember(e => e.Id).SetIdGenerator(CombGuidGenerator.Instance);
            map.MapMember(e => e.DoctorId);
            map.MapMember(e => e.OfficeHours);
            map.MapMember(e => e.Appointments);
        });
    }
}
