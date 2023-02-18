using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Data.Mongodb.Core;
using RuiSantos.ZocDoc.Data.Mongodb.Entities;

namespace RuiSantos.ZocDoc.Data.Mongodb.Mappings;

internal class DoctorClassMap : IRegisterClassMap
{
    public void Register()
    {
        BsonClassMap.RegisterClassMap<Doctor>(map =>
        {
            map.AutoMap();
            map.SetDiscriminator(DoctorEntity.Discriminator);

            map.MapIdMember(e => e.Id).SetIdGenerator(CombGuidGenerator.Instance);
            map.MapMember(e => e.License);
            map.MapMember(e => e.Specialties);
        });
    }
}

