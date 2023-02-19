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
}

