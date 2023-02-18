using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Data.Mongodb.Core;

namespace RuiSantos.ZocDoc.Data.Mongodb.Mappings;

internal class AppointmentClassMap: IRegisterClassMap
{		
    public void Register()
    {
        BsonClassMap.RegisterClassMap<Appointment>(map =>
        {
            map.AutoMap();
            map.MapMember(e => e.Id);
            map.MapMember(e => e.Date);
        });
    }
}

