using MongoDB.Bson.Serialization;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Data.Mongodb.Core.Interfaces;

namespace RuiSantos.ZocDoc.Data.Mongodb.Mappings;

internal sealed class PersonClassMap : IRegisterClassMap
{
    public void Register()
    {
        BsonClassMap.RegisterClassMap<Person>(map =>
        {
            map.AutoMap();

            map.MapMember(e => e.Email);
            map.MapMember(e => e.FirstName);
            map.MapMember(e => e.LastName);
            map.MapMember(e => e.ContactNumbers);
        });
    }
}