using RuiSantos.Labs.Domain.Commands;
using RuiSantos.Labs.Services.Commands.Doctors;
using RuiSantos.Labs.Services.Endpoints.Schemas;

namespace RuiSantos.Labs.Services.Commands;

internal interface IDoctorsCommandFactory
{
    IAsyncCommand<GetDoctorResponse?> CreateGetDoctorByIdCommand(GetDoctorByIdCommandInput input);
    IAsyncCommand<GetDoctorResponse?> CreateGetDoctorByLicenseCommand(GetDoctorByLicenseCommandInput input);
    IAsyncCommand<GetDoctorsResponse> CreateGetDoctorsCommand(GetDoctorsCommandInput input);
    IAsyncCommand CreateStoreDoctorCommand(StoreDoctorRequest request);
}

internal class DoctorsCommandFactory(IServiceProvider provider): IDoctorsCommandFactory
{
    public IAsyncCommand<GetDoctorResponse?> CreateGetDoctorByIdCommand(GetDoctorByIdCommandInput input)
        => ActivatorUtilities.CreateInstance<GetDoctorByIdCommand>(provider, input);

    public IAsyncCommand<GetDoctorResponse?> CreateGetDoctorByLicenseCommand(GetDoctorByLicenseCommandInput input)
        => ActivatorUtilities.CreateInstance<GetDoctorByLicenseCommand>(provider, input);

    public IAsyncCommand<GetDoctorsResponse> CreateGetDoctorsCommand(GetDoctorsCommandInput input)
        => ActivatorUtilities.CreateInstance<GetDoctorsCommand>(provider, input);

    public IAsyncCommand CreateStoreDoctorCommand(StoreDoctorRequest request)
        => ActivatorUtilities.CreateInstance<StoreDoctorCommand>(provider, request);
}