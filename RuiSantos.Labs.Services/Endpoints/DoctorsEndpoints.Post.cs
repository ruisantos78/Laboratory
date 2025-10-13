using RuiSantos.Labs.Services.Commands;
using RuiSantos.Labs.Services.Endpoints.Schemas;
using RuiSantos.Labs.Services.Extensions;

namespace RuiSantos.Labs.Services.Endpoints;

partial class DoctorsEndpoints
{
    private static async Task<IResult> StoreDoctor(StoreDoctorRequest request    
        , IDoctorsCommandFactory commandFactory
        , CancellationToken cancellationToken)
    {
        try
        {
            var command = commandFactory.CreateStoreDoctorCommand(request);
            await command.ExecuteAsync(cancellationToken);

            return Results.NoContent();
        }
        catch (Exception ex)
        {
            return ex.ToHttpResult();
        }
    }
}
