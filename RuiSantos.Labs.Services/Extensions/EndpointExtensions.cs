namespace RuiSantos.Labs.Services.Extensions;

internal static class EndpointExtensions
{
    public static IResult ToHttpResult(this Exception ex)
    {
        return ex switch
        {
            FluentValidation.ValidationException vex => vex.ToValidationProblem(),
            _ => Results.Problem(ex.Message, statusCode: 500)
        };
    }

    private static IResult ToValidationProblem(this FluentValidation.ValidationException ex)
    {
        var errors = ex.Errors
            .GroupBy(x => x.PropertyName)
            .ToDictionary(
                k => k.Key, 
                v => v.Select(x => x.ErrorMessage).ToArray()
            );
        
        return Results.ValidationProblem(errors);
    }
}