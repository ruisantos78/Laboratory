using RuiSantos.Labs.Services.Commands;
using RuiSantos.Labs.Services.Commands.Doctors;
using RuiSantos.Labs.Services.Extensions;

namespace RuiSantos.Labs.Services.Endpoints;

partial class DoctorsEndpoints
{
    private static async Task<IResult> GetDoctors(int? take, string? from
        , IDoctorsCommandFactory commandFactory
        , CancellationToken cancellationToken)
    {
        try
        {
            var command = commandFactory.CreateGetDoctorsCommand(new GetDoctorsCommandInput()
            {
                Take = take,
                PaginationToken = from
            });

            var result = await command.ExecuteAsync(cancellationToken);
            if (result.Items.Length == 0)
                return Results.NoContent();

            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            return ex.ToHttpResult();
        }
    }
    
    private static async Task<IResult> GetDoctorById(string id
        , IDoctorsCommandFactory commandFactory
        , CancellationToken cancellationToken)
    {
        try
        {
            var command = commandFactory.CreateGetDoctorByIdCommand(new GetDoctorByIdCommandInput()
            {
                DoctorId = id
            });

            var result = await command.ExecuteAsync(cancellationToken);
            if (result is null)
                return Results.NoContent();

            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            return ex.ToHttpResult();
        }
    }
}
