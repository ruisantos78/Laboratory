using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using RuiSantos.ZocDoc.Data.Mongodb.Core;
using RuiSantos.ZocDoc.Data.Mongodb.Entities;

namespace RuiSantos.ZocDoc.Data.Mongodb.Mappings;

internal class MedicalSpecialityClassMap : IRegisterClassMap
{
    public void Register()
    {
        BsonClassMap.RegisterClassMap<MedicalSpecialityEntity>(map =>
        {
            map.AutoMap();
            map.SetDiscriminator(MedicalSpecialityEntity.Discriminator);

            map.MapIdMember(e => e.Id).SetIdGenerator(StringObjectIdGenerator.Instance);

            map.MapMember(e => e.Description);
        });
    }
}
