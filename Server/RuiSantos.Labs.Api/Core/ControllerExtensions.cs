using System.Net;
using Microsoft.AspNetCore.Mvc;
using RuiSantos.Labs.Core.Services.Exceptions;

namespace RuiSantos.Labs.Api.Core;

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
    public static IActionResult Failure(this Controller controller, Exception exception)
    {
        return exception switch
        {
            ServiceFailException managementFailException => controller.Problem(managementFailException.Message),
            ValidationFailException validationFailException => controller.BadRequest(validationFailException.Message),
            _ => controller.Problem(),
        };
    }

    public static IActionResult Success(this Controller controller, bool created = false)
    {
        if (created)
            return controller.StatusCode((int)HttpStatusCode.Created);

        return controller.Ok();
    }

    /// <summary>
    /// Handles the successful result of the controller.
    /// Returns a 200 OK if the result is not null, or 204 No Content otherwise.
    /// </summary>
    /// <param name="controller">The controller.</param>
    /// <param name="result">The result.</param>
    /// <param name="contractType">The contract type. (optional)</param>
    /// <returns>The result HTTP Status code and the result.</returns>
    public static IActionResult Success<TModel>(this Controller controller, TModel? result, Type? contractType = null)
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
    /// <param name="models">The model list.</param>
    /// <param name="contractType">The contract type. (optional)</param>
    /// <returns>The result HTTP Status code and the result.</returns>
    public static IActionResult Success<TModel>(this Controller controller, IEnumerable<TModel>? models, Type? contractType = null)
    {
        if (contractType is null)
            return controller.Ok(models);

        var values = models?.ToArray();
        if (values?.Any() is not true)
            return controller.NoContent();

        var result = values.Select(model => Activator.CreateInstance(contractType, model));
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
    public static async Task<IActionResult> SuccessAsync<TModel>(this Controller controller, IAsyncEnumerable<TModel> model, Type? contractType = null)
    {
        var response = await model.ToArrayAsync();
        if (!response.Any())
            return controller.NoContent();

        if (contractType is null)
            return controller.Ok(response);

        var result = response.Select(item => Activator.CreateInstance(contractType, item));

        return controller.Ok(result);
    }
}

