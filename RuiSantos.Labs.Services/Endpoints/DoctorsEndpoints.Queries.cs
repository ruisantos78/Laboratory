using RuiSantos.Labs.Services.Commands;
using RuiSantos.Labs.Services.Commands.Doctors;
using RuiSantos.Labs.Services.Extensions;

namespace RuiSantos.Labs.Services.Endpoints;

internal partial class DoctorsEndpoints
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
            return result.Items.Length == 0 ? Results.NoContent() : Results.Ok(result);
        }
        catch (Exception ex)
        {
            return ex.ToHttpResult();
        }
    }
    
    private static async Task<IResult> GetDoctorsBySpecialization(string specialization
        , IDoctorsCommandFactory commandFactory
        , CancellationToken cancellationToken)
    {
        try
        {
            var command = commandFactory.CreateGetDoctorsBySpecializationCommand(new GetDoctorsBySpecializationCommandInput()
            {
                Specialization = specialization
            });

            var result = await command.ExecuteAsync(cancellationToken);
            return result.Items.Length == 0 ? Results.NoContent() : Results.Ok(result);
        }
        catch (Exception ex)
        {
            return ex.ToHttpResult();
        }
    }
    
    private static async Task<IResult> GetDoctorById(string? id
        , IDoctorsCommandFactory commandFactory
        , CancellationToken cancellationToken)
    {
        try
        {
            var command = commandFactory.CreateGetDoctorByIdCommand(new GetDoctorByIdCommandInput()
            {
                DoctorId = id ?? string.Empty
            });

            var result = await command.ExecuteAsync(cancellationToken);
            return result is null ? Results.NoContent() : Results.Ok(result);
        }
        catch (Exception ex)
        {
            return ex.ToHttpResult();
        }
    }
    
    private static async Task<IResult> GetDoctorByLicense(string license
        , IDoctorsCommandFactory commandFactory
        , CancellationToken cancellationToken)
    {
        try
        {
            var command = commandFactory.CreateGetDoctorByLicenseCommand(new GetDoctorByLicenseCommandInput()
            {
                License = license
            });

            var result = await command.ExecuteAsync(cancellationToken);
            return result is null ? Results.NoContent() : Results.Ok(result);
        }
        catch (Exception ex)
        {
            return ex.ToHttpResult();
        }
    }
}
