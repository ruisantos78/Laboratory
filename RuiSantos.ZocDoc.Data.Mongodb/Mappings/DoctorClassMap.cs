using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using RuiSantos.ZocDoc.Core.Models;

namespace RuiSantos.ZocDoc.Data.Mongodb.Mappings;

internal sealed class DoctorClassMap : IRegisterClassMap
{
    public const string Discriminator = "Doctors";
    
    public void Register()
    {
        BsonClassMap.RegisterClassMap<Doctor>(map =>
        {
            map.AutoMap();
            map.SetDiscriminator(Discriminator);

            map.MapIdProperty(e => e.Id).SetIdGenerator(CombGuidGenerator.Instance);

            map.MapMember(e => e.License);
            map.MapMember(e => e.Specialities);
            map.MapMember(e => e.OfficeHours);
            map.MapMember(e => e.Appointments);
        });
    }
}

