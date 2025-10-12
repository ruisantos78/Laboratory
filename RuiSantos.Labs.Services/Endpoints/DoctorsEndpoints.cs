using RuiSantos.Labs.Services.Commands;
using RuiSantos.Labs.Services.Commands.Doctors;
using RuiSantos.Labs.Services.Endpoints.Converters;
using RuiSantos.Labs.Services.Endpoints.Schemas;
using RuiSantos.Labs.Services.Extensions;

namespace RuiSantos.Labs.Services.Endpoints;

internal static class DoctorsEndpoints
{
    public static void MapDoctorsEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/doctors", GetDoctors)
            .Produces<GetDoctorsResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest);
    }
    
    private static async Task<IResult> GetDoctors(int? take, string? from
        , IDoctorsCommandFactory commandFactory
        , CancellationToken cancellationToken)
    {
        try
        {
            var command = commandFactory.GetDoctorsCommand(new GetDoctorsCommandInput()
            {
                Take = take,
                PaginationToken = from
            });

            var result = await command.Execute(cancellationToken);
            
            return result.Models.Count == 0 
                ? Results.NoContent() 
                : Results.Ok(result.ToGetDoctorsResponse());
        }
        catch (Exception ex)
        {
            return ex.ToHttpResult();
        }
    }
}