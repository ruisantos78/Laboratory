using FluentValidation;
using RuiSantos.Labs.Core.Services.Exceptions;

namespace RuiSantos.Labs.Core.Validators;

internal static class Validator
{
    private static AbstractValidator<TModel> GetValidator<TModel>(params object?[] args)
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

    public static void ThrowExceptionIfIsNotValid<TModel>(TModel model, params object?[] args) where TModel: class {
        var validation = Validator.GetValidator<TModel>(args).Validate(model);
        if (!validation.IsValid)
        {
            throw new ValidationFailException(validation.Errors);
        }
    }
}
