using RuiSantos.Labs.Services.Endpoints.Schemas;

namespace RuiSantos.Labs.Services.Endpoints;

internal static partial class DoctorsEndpoints
{
    public static void MapDoctorsEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/doctors", GetDoctors)
            .Produces<GetDoctorsResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        routes.MapGet("/doctor/{id}", GetDoctorById)
            .Produces<GetDoctorResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
        
        routes.MapPost("/doctor", StoreDoctor)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);   
    }    
}