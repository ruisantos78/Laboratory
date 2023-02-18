using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using RuiSantos.ZocDoc.Data.Mongodb.Core;
using RuiSantos.ZocDoc.Data.Mongodb.Entities;

namespace RuiSantos.ZocDoc.Data.Mongodb.Mappings;

internal class PatientClassMap: IRegisterClassMap
{
    public void Register()
    {
        BsonClassMap.RegisterClassMap<PatientEntity>(map =>
        {
            map.AutoMap();
            map.SetDiscriminator(PatientEntity.Discriminator);

            map.MapIdMember(e => e.Id).SetIdGenerator(StringObjectIdGenerator.Instance);

            map.MapMember(e => e.SocialSecurityNumber);
        });
    }
}