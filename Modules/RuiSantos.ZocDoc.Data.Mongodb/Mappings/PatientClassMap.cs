using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using RuiSantos.ZocDoc.Core.Models;

namespace RuiSantos.ZocDoc.Data.Mongodb.Mappings;

internal sealed class PatientClassMap : IRegisterClassMap
{
    public const string Discriminator = "Patients";

    public void Register()
    {
        BsonClassMap.RegisterClassMap<Patient>(map =>
        {
            map.AutoMap();
            map.SetDiscriminator(Discriminator);

            map.MapIdProperty(e => e.Id).SetIdGenerator(CombGuidGenerator.Instance);

            map.MapMember(e => e.SocialSecurityNumber);
            map.MapMember(e => e.Appointments);
        });
    }
}