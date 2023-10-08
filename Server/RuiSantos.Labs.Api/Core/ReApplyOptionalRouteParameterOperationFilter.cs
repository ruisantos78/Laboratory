using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.RegularExpressions;

namespace RuiSantos.Labs.Api.Core;

internal class ReApplyOptionalRouteParameterOperationFilter : IOperationFilter
{
    const string CaptureName = "routeParameter";

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var httpMethodAttributes = context.MethodInfo
            .GetCustomAttributes(true)
            .OfType<Microsoft.AspNetCore.Mvc.Routing.HttpMethodAttribute>();

        var httpMethodWithOptional = httpMethodAttributes.FirstOrDefault(m => m.Template?.Contains('?') ?? false);
        if (httpMethodWithOptional?.Template is null)
            return;

        string regex = $"{{(?<{CaptureName}>\\w+)\\?}}";

        var matches = Regex.Matches(httpMethodWithOptional.Template, regex);

        foreach (Match match in matches.Cast<Match>())
        {
            var name = match.Groups[CaptureName].Value;

            var parameter = operation.Parameters.FirstOrDefault(p => p.In == ParameterLocation.Path && p.Name == name);
            if (parameter != null)
            {
                parameter.AllowEmptyValue = true;
                parameter.Description =
                    "Must check \"Send empty value\" or Swagger passes a comma for empty values otherwise";
                parameter.Required = false;
                parameter.Schema.Nullable = true;
            }
        }
    }
}