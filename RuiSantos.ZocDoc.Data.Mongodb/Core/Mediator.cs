using MongoDB.Bson.Serialization;

namespace RuiSantos.ZocDoc.Data.Mongodb.Core;

internal static class Mediator
{
    public static IEntity<TModel>? GetEntity<TModel>()
    {
        var entityType = typeof(IEntity<>).MakeGenericType(typeof(TModel));
        var entityImplType = entityType.Assembly.GetTypes()
            .FirstOrDefault(entityType.IsAssignableFrom);

        if (entityImplType is null)
            return default;

        if (Activator.CreateInstance(entityImplType) is IEntity<TModel> entity)
            return entity;

        return default;
    }

    public static void RegisterClassMaps()
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

