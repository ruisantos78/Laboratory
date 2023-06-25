using Microsoft.AspNetCore.Mvc;
using RuiSantos.ZocDoc.Core.Managers.Exceptions;

namespace RuiSantos.ZocDoc.Api.Core;

/// <summary>
/// Extensions for the Controller class.
/// </summary>
internal static class ControllerExtensions
{
    /// <sumary>
    /// Handles the exceptions thrown by the controller. 
    /// Returns a 400 Bad Request if the exception is a ValidationFailException or a 500 Internal Server Error otherwise. 
    /// </sumary>
    /// <param name="controller">The controller.</param>
    /// <param name="exception">The exception.</param>
    /// <returns>The result HTTP Status code and the message of the exception, if any, that caused the failure of the handling.</returns>
    public static IActionResult FromException(this Controller controller, Exception exception)
    {
        return exception switch
        {
            ManagementFailException managementFailException => controller.Problem(managementFailException.Message),
            ValidationFailException validationFailException => controller.BadRequest(validationFailException.Message),
            _ => controller.Problem(),
        };
    }

    /// <summary>
    /// Handles the successful result of the controller.
    /// Returns a 200 OK if the result is not null, or 204 No Content otherwise.
    /// </summary>
    /// <param name="controller">The controller.</param>
    /// <param name="result">The result.</param>
    /// <param name="contractType">The contract type. (optional)</param>
    /// <returns>The result HTTP Status code and the result.</returns>
    public static IActionResult OkOrNoContent<TModel>(this Controller controller, TModel? result, Type? contractType = null)
    {
        if (result is null)
            return controller.NoContent();

        if (contractType is null)
            return controller.Ok(result);

        var contract = Activator.CreateInstance(contractType, result);
        if (contract is null)
            return controller.NoContent();

        return controller.Ok(contract);
    }

    /// <summary>
    /// Handles the successful result collection of the controller.
    /// Returns a 200 OK if the result collection is not null or empty, or 204 No Content otherwise.
    /// </summary>
    /// <param name="controller">The controller.</param>
    /// <param name="model">The model.</param>
    /// <param name="contractType">The contract type. (optional)</param>
    /// <returns>The result HTTP Status code and the result.</returns>
    public static IActionResult OkOrNoContent<TModel>(this Controller controller, IEnumerable<TModel> model, Type? contractType = null)
    {
        var response = model.ToArray();
        if (!response.Any())
            return controller.NoContent();

        if (contractType is null)
            return controller.Ok(response);

        var result = Array.CreateInstance(contractType, response.Length);
        var i = 0;
        foreach (var item in response)
        {
            var contract = Activator.CreateInstance(contractType, item); 
            if (contract is not null)
                result.SetValue(contract, i++);
        }
        
        return controller.Ok(result);
    }

    /// <summary>
    /// Handles the successful result collection of the controller.
    /// Returns a 200 OK if the result collection is not null or empty, or 204 No Content otherwise.
    /// </summary>
    /// <param name="controller">The controller.</param>
    /// <param name="model">The model.</param>
    /// <param name="contractType">The contract type. (optional)</param>
    /// <returns>The result HTTP Status code and the result.</returns>    
    public static async Task<IActionResult> OkOrNoCotentAsync<TModel>(this Controller controller, IAsyncEnumerable<TModel> model, Type? contractType = null)
    {
        var response = await model.ToArrayAsync();
        if (!response.Any())
            return controller.NoContent();

        if (contractType is null)
            return controller.Ok(response);

        var i = 0;
        var result = Array.CreateInstance(contractType, response.Length);
        foreach (var item in response)
        {
            var contract = Activator.CreateInstance(contractType, item);
            if (contract is not null)
                result.SetValue(contract, i++);
        }

        return controller.Ok(result);
    }
}

