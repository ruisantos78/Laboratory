using Microsoft.Extensions.Logging;
using RuiSantos.Labs.Core.Extensions;
using RuiSantos.Labs.Core.Models;
using RuiSantos.Labs.Core.Repositories;
using RuiSantos.Labs.Core.Resources;
using RuiSantos.Labs.Core.Services.Exceptions;

namespace RuiSantos.Labs.Core.Commands.DoctorCommands;

internal class GetDoctorAppointmentsByDateCommand(GetDoctorAppointmentsByDateCommandInput input
    , IDoctorRepository doctorRepository
    , IAppointmentsRepository appointmentsRepository
    , ILogger<GetDoctorAppointmentsByDateCommand> logger
    ): IAsyncCommand<PatientAppointment[]>
{
    public async Task<PatientAppointment[]> Execute(CancellationToken cancellationToken = default)
    {
        try
        {
            if (await doctorRepository.FindAsync(input.DoctorId, cancellationToken) is not { } doctor)
                return [];

            var result = await appointmentsRepository.GetPatientAppointmentsAsync(doctor, input.Date, cancellationToken);
            return [..result];
        }
        catch (Exception ex)
        {
            logger.Fail(ex);
            throw new ServiceFailException(MessageResources.DoctorsGetAppointmentsFail);
        }
    }
}
//Guid doctorId, DateTime? dateTime
internal class GetDoctorAppointmentsByDateCommandInput
{
    public Guid DoctorId { get; init; }
    public DateOnly Date { get; init; }
}