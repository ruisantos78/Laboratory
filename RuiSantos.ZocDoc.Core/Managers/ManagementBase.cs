using FluentValidation;
using RuiSantos.ZocDoc.Core.Managers.Exceptions;

namespace RuiSantos.ZocDoc.Core.Managers;

public abstract class ManagementBase
{
    public static bool IsValid<TValidator, TModel>(TModel model, TValidator validator, out ValidationFailException exception)
        where TValidator : AbstractValidator<TModel>
    {
        var validation = validator.Validate(model);
        if (!validation.IsValid)
        {
            exception = new ValidationFailException(validation.Errors);
            return false;
        }

        exception = ValidationFailException.Empty;
        return true;
    }
}

