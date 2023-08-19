using FluentValidation;

namespace RuiSantos.ZocDoc.Core.Resources;

internal static class Mediators
{
    public static AbstractValidator<TModel> GetValidator<TModel>(params object?[] args)
        where TModel : class
    {
        var entityType = typeof(AbstractValidator<>).MakeGenericType(typeof(TModel));

        var entityImplType = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .FirstOrDefault(entityType.IsAssignableFrom);

        if (entityImplType is not null && Activator.CreateInstance(entityImplType, args) is AbstractValidator<TModel> entity)
            return entity;

        throw new ArgumentException($"Cannot find a validator for {typeof(TModel).Name}");
    }
}
