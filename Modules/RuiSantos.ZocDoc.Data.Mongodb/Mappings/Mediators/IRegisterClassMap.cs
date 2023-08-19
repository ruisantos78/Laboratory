using MongoDB.Bson.Serialization;
using RuiSantos.ZocDoc.Data.Mongodb.Mappings.Serializers;

namespace RuiSantos.ZocDoc.Data.Mongodb.Mappings;

internal interface IRegisterClassMap
{
    void Register();
}

internal static class RegisterClassMaps
{
    public static void InitializeDatabase()
    {
        BsonSerializer.RegisterSerializer(typeof(DateOnly), DateOnlySerializer.Instance);
        BsonSerializer.RegisterSerializer(typeof(TimeOnly), TimeOnlySerializer.Instance);

        var mappings = typeof(IRegisterClassMap).Assembly.GetTypes()
            .Where(t => !t.IsInterface && t.IsAssignableFrom(typeof(IRegisterClassMap)));

        foreach (var mapping in mappings)
        {
            if (Activator.CreateInstance(mapping) is IRegisterClassMap instance)
                instance.Register();
        }
    }
}
