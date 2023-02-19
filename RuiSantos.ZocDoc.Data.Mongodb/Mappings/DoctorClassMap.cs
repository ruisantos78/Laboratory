using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Data.Mongodb.Core.Interfaces;
using RuiSantos.ZocDoc.Data.Mongodb.Entities;

namespace RuiSantos.ZocDoc.Data.Mongodb.Mappings;

internal sealed class DoctorClassMap : IRegisterClassMap
{
    public void Register()
    {
        BsonClassMap.RegisterClassMap<Doctor>(map =>
        {
            map.AutoMap();
            map.SetDiscriminator(DoctorEntity.Discriminator);

            map.MapIdProperty(e => e.Id).SetIdGenerator(CombGuidGenerator.Instance);

            map.MapMember(e => e.License);
            map.MapMember(e => e.Specialties);
            map.MapMember(e => e.OfficeHours);
            map.MapMember(e => e.Appointments);
        });
    }
}

