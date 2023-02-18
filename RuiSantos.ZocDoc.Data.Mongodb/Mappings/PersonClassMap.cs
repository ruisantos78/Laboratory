using System;
using MongoDB.Bson.Serialization;
using RuiSantos.ZocDoc.Data.Mongodb.Core;
using RuiSantos.ZocDoc.Core.Models;

namespace RuiSantos.ZocDoc.Data.Mongodb.Mappings;

internal class PersonClassMap : IRegisterClassMap
{
    public void Register()
    {
        BsonClassMap.RegisterClassMap<Patient>(map =>
        {
            map.AutoMap();
            map.MapMember(e => e.Email);
            map.MapMember(e => e.FirstName);
            map.MapMember(e => e.LastName);
            map.MapMember(e => e.ContactNumbers);
        });
    }
}