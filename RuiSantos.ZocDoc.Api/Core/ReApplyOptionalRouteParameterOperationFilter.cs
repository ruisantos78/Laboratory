using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace RuiSantos.ZocDoc.Api.Core;

public class ReApplyOptionalRouteParameterOperationFilter : IOperationFilter
{
    const string captureName = "routeParameter";

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var httpMethodAttributes = context.MethodInfo
            .GetCustomAttributes(true)
            .OfType<Microsoft.AspNetCore.Mvc.Routing.HttpMethodAttribute>();

        var httpMethodWithOptional = httpMethodAttributes?.FirstOrDefault(m => m.Template?.Contains("?") ?? false);
        if (httpMethodWithOptional?.Template is null)
            return;

        string regex = $"{{(?<{captureName}>\\w+)\\?}}";
        
        var matches = System.Text.RegularExpressions.Regex.Matches(httpMethodWithOptional.Template, regex);

        foreach (System.Text.RegularExpressions.Match match in matches)
        {
            var name = match.Groups[captureName].Value;

            var parameter = operation.Parameters.FirstOrDefault(p => p.In == ParameterLocation.Path && p.Name == name);
            if (parameter != null)
            {
                parameter.AllowEmptyValue = true;
                parameter.Description = "Must check \"Send empty value\" or Swagger passes a comma for empty values otherwise";
                parameter.Required = false;
                parameter.Schema.Nullable = true;
            }
        }
    }
}