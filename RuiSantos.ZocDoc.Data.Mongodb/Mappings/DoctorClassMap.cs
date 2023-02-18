using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using RuiSantos.ZocDoc.Data.Mongodb.Core;
using RuiSantos.ZocDoc.Data.Mongodb.Entities;

namespace RuiSantos.ZocDoc.Data.Mongodb.Mappings;

internal class DoctorClassMap : IRegisterClassMap
{
    public void Register()
    {
        BsonClassMap.RegisterClassMap<DoctorEntity>(map =>
        {
            map.AutoMap();
            map.SetDiscriminator(DoctorEntity.Discriminator);

            map.MapIdMember(e => e.Id).SetIdGenerator(StringObjectIdGenerator.Instance);

            map.MapMember(e => e.License);
            map.MapMember(e => e.Specialities);
        });
    }
}

