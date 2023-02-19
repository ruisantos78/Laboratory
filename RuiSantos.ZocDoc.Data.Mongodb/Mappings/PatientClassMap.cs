using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Data.Mongodb.Core.Interfaces;
using RuiSantos.ZocDoc.Data.Mongodb.Entities;

namespace RuiSantos.ZocDoc.Data.Mongodb.Mappings;

internal sealed class PatientClassMap: IRegisterClassMap
{
    public void Register()
    {
        BsonClassMap.RegisterClassMap<Patient>(map =>
        {
            map.AutoMap();
            map.SetDiscriminator(PatientEntity.Discriminator);

            map.MapIdMember(e => e.Id).SetIdGenerator(CombGuidGenerator.Instance);

            map.MapMember(e => e.SocialSecurityNumber);
            map.MapMember(e => e.Appointments);
        });
    }
}