using MongoDB.Bson.Serialization;
using RuiSantos.ZocDoc.Core.Models;

namespace RuiSantos.ZocDoc.Data.Mongodb.Mappings;

internal sealed class AppointmentClassMap : IRegisterClassMap
{
    public void Register()
    {
        BsonClassMap.RegisterClassMap<Appointment>(map =>
        {
            map.AutoMap();

            map.MapMember(e => e.Id);
            map.MapMember(e => e.Week);
            map.MapMember(e => e.Date);
            map.MapMember(e => e.Time);
        });
    }
}

