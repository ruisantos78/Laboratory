using Microsoft.AspNetCore.Mvc;
using RuiSantos.ZocDoc.Core.Managers;

namespace RuiSantos.ZocDoc.Api.Core;

internal static class ControllerExtensions
{
    public static IActionResult FromTask(this Controller controller, Task task)
    {
        if (task.IsCompletedSuccessfully)
            return controller.Ok();

        if (task.IsCanceled)
            return controller.NoContent();

        if (task.IsFaulted)
        {
            if (task.Exception?.InnerExceptions.OfType<IFailure>().FirstOrDefault() is Exception fail)
                return controller.FromException(fail);

            if (task.Exception?.InnerException is Exception innerException)
                return controller.FromException(innerException);            

            return controller.Problem();
        }

        return controller.NotFound();
    }

    public static IActionResult FromException(this Controller controller, Exception exception)
    {
        switch (exception)
        {
            case ManagementFailException managementFailException:
                return controller.Problem(managementFailException.Message);

            case ValidationFailException validationFailException:
                return controller.BadRequest(validationFailException.Message);

            default:
                return controller.Problem();
        }
    }
}

