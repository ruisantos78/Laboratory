using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using RuiSantos.ZocDoc.Core.Models;

namespace RuiSantos.ZocDoc.Data.Mongodb.Mappings;

internal sealed class MedicalSpecialityClassMap : IRegisterClassMap
{
    public const string Discriminator = "MedicalSpecialities";

    public void Register()
    {
        BsonClassMap.RegisterClassMap<MedicalSpecialty>(map =>
        {
            map.AutoMap();
            map.SetDiscriminator(Discriminator);

            map.MapIdProperty(e => e.Id).SetIdGenerator(CombGuidGenerator.Instance);

            map.MapMember(e => e.Description);
        });
    }
}
