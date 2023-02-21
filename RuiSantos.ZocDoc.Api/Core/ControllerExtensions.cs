using Microsoft.AspNetCore.Mvc;
using RuiSantos.ZocDoc.Core.Managers.Exceptions;

namespace RuiSantos.ZocDoc.Api.Core;

internal static class ControllerExtensions
{
    public static IActionResult FromException(this Controller controller, Exception exception)
    {
        return exception switch
        {
            ManagementFailException managementFailException => controller.Problem(managementFailException.Message),
            ValidationFailException validationFailException => controller.BadRequest(validationFailException.Message),
            _ => controller.Problem(),
        };
    }

    public static IActionResult OkOrNotFound<TResult>(this Controller controller, IEnumerable<TResult> result)
    {
        if (result is null || !result.Any())
            return controller.NotFound();

        return controller.Ok(result);
    }

    public static IActionResult OkOrNotFound<TContract>(this Controller controller, object? model)
    {
        if (model is null)
            return controller.NotFound();

        if (Activator.CreateInstance(typeof(TContract), model) is TContract result)
            return controller.Ok(result);

        throw new ManagementFailException($"Fail to create an instance of {typeof(TContract).Name}");
    }
}

