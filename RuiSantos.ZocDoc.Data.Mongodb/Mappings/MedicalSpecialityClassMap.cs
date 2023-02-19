using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Data.Mongodb.Core.Interfaces;
using RuiSantos.ZocDoc.Data.Mongodb.Entities;

namespace RuiSantos.ZocDoc.Data.Mongodb.Mappings;

internal sealed class MedicalSpecialityClassMap : IRegisterClassMap
{
    public void Register()
    {
        BsonClassMap.RegisterClassMap<MedicalSpeciality>(map =>
        {
            map.AutoMap();
            map.SetDiscriminator(MedicalSpecialityEntity.Discriminator);

            map.MapIdProperty(e => e.Id).SetIdGenerator(CombGuidGenerator.Instance);

            map.MapMember(e => e.Description);
        });
    }
}
