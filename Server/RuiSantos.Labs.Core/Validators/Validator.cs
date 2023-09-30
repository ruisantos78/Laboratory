using FluentValidation;
using RuiSantos.Labs.Core.Services.Exceptions;

namespace RuiSantos.Labs.Core.Validators;

internal static class Validator
{
    private static AbstractValidator<TModel> GetValidator<TModel>(params object?[] args)
        where TModel : class
    {
        var abstractValidatorType = typeof(AbstractValidator<>).MakeGenericType(typeof(TModel));
        var validatorType = typeof(Validator).Assembly.GetTypes().First(abstractValidatorType.IsAssignableFrom);

        if (Activator.CreateInstance(validatorType, args) is not AbstractValidator<TModel> entity)
            throw new ArgumentException($"Cannot find a validator for {typeof(TModel).Name}");

        return entity;
    }

    public static void ThrowExceptionIfIsNotValid<TModel>(TModel model, params object?[] args)
        where TModel: class
    {
        var validation = GetValidator<TModel>(args).Validate(model);
        if (!validation.IsValid)
            throw new ValidationFailException(validation.Errors);
    }
}
