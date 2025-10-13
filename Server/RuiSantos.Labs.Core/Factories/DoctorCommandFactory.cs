using Microsoft.Extensions.DependencyInjection;
using RuiSantos.Labs.Core.Commands.DoctorCommands;
using RuiSantos.Labs.Domain.Commands;
using RuiSantos.Labs.Domain.Models;

namespace RuiSantos.Labs.Core.Factories;

internal class DoctorCommandFactory(IServiceProvider provider)
{
    public IAsyncCommand CreateDoctorCommand(CreateDoctorCommandInput input)
        => ActivatorUtilities.CreateInstance<CreateDoctorCommand>(provider, input);

    public IAsyncCommand UpdateOfficeHoursCommand(UpdateOfficeHoursCommandInput input)
        => ActivatorUtilities.CreateInstance<UpdateOfficeHoursCommand>(provider, input);

    public IAsyncCommand<PatientAppointment[]> GetDoctorAppointmentsByDateCommand(GetDoctorAppointmentsByDateCommandInput input)
        => ActivatorUtilities.CreateInstance<GetDoctorAppointmentsByDateCommand>(provider, input);

    public IAsyncCommand<Pagination<Doctor>> GetAllDoctorsCommand(GetAllDoctorsCommandInput input)
        => ActivatorUtilities.CreateInstance<GetAllDoctorsCommand>(provider, input);

    public IAsyncCommand<Doctor?> GetDoctorByLicenseCommand(GetDoctorByLicenseCommandInput input)
        => ActivatorUtilities.CreateInstance<GetDoctorByLicenseCommand>(provider, input);
}