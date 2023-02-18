namespace RuiSantos.ZocDoc.Data.Mongodb.Core;

internal class Mediator
{
    public static IEntity<TModel>? GetEntity<TModel>()
    {
        var entityType = typeof(IEntity<>).MakeGenericType(typeof(TModel));
        var entityImplType = entityType.Assembly.GetTypes()
            .FirstOrDefault(t => entityType.IsAssignableFrom(t));

        if (entityImplType is null)
            return default;

        if (Activator.CreateInstance(entityImplType) is IEntity<TModel> entity)
            return entity;

        return default;
    }    
}

