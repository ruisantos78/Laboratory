using RuiSantos.ZocDoc.Core.Managers.Exceptions;

namespace RuiSantos.ZocDoc.Core.Resources;

internal static class ManagementUtils
{
    public static bool IsValid<TModel>(TModel model, out ValidationFailException exception, params object?[] args)
        where TModel : class
    {
        var validation = Mediators.GetValidator<TModel>(args).Validate(model);
        if (!validation.IsValid)
        {
            exception = new ValidationFailException(validation.Errors);
            return false;
        }

        exception = ValidationFailException.Empty;
        return true;
    }
}
