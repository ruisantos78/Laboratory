using RuiSantos.ZocDoc.Core.Managers.Exceptions;

namespace RuiSantos.ZocDoc.Core.Resources;

internal static class ManagementUtils
{
    public static void ThrowExceptionIfIsNotValid<TModel>(TModel model, params object?[] args) where TModel: class {
        var validation = Mediators.GetValidator<TModel>(args).Validate(model);
        if (!validation.IsValid)
        {
            throw new ValidationFailException(validation.Errors);
        }
    }
}
