using Microsoft.Extensions.Logging;
using RuiSantos.Labs.Core.Extensions;
using RuiSantos.Labs.Core.Models;
using RuiSantos.Labs.Core.Repositories;
using RuiSantos.Labs.Core.Resources;
using RuiSantos.Labs.Core.Services.Exceptions;
using RuiSantos.Labs.Domain.Extensions.Models;
using RuiSantos.Labs.Domain.Extensions.Repositories;

namespace RuiSantos.Labs.Core.Services;

internal class AppointmentService(
    IDoctorRepository doctorRepository,
    IPatientRepository patientRepository,
    IAppointmentsRepository appointmentsRepository,
    ILogger<AppointmentService> logger)
    : IAppointmentService
{
    private readonly ILogger _logger = logger;

    public async Task CreateAppointmentAsync(string socialNumber, Guid doctorId, DateTime dateTime,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (await doctorRepository.FindAsync(doctorId, cancellationToken) is not { } doctor)
                throw new ValidationFailException(MessageResources.DoctorLicenseNotFound);

            if (await appointmentsRepository.GetAsync(doctor, dateTime) is not null)
                throw new ValidationFailException(MessageResources.RecordAlreadyExists);

            if (await patientRepository.FindAsync(socialNumber) is not { } patient)
                throw new ValidationFailException(MessageResources.PatientSocialNumberNotFound);

            if (await appointmentsRepository.GetAsync(patient, dateTime) is not null)
                throw new ValidationFailException(MessageResources.RecordAlreadyExists);

            await appointmentsRepository.StoreAsync(doctor, patient, dateTime);
        }
        catch (ValidationFailException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.Fail(ex);
            throw new ServiceFailException(MessageResources.PatientSetFail);
        }
    }

    public async Task DeleteAppointmentAsync(string socialNumber, Guid doctorId, DateTime dateTime,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (await patientRepository.FindAsync(socialNumber) is not { } patient)
                throw new ValidationFailException(MessageResources.PatientSocialNumberNotFound);

            if (await appointmentsRepository.GetAsync(patient, dateTime) is not { } patientAppointment)
                return;

            if (await doctorRepository.FindAsync(doctorId, cancellationToken) is not { } doctor)
                return;

            if (await appointmentsRepository.GetAsync(doctor, dateTime) is not { } doctorAppointment)
                return;

            if (patientAppointment.Id != doctorAppointment.Id)
                return;

            await appointmentsRepository.RemoveAsync(patientAppointment);
        }
        catch (ValidationFailException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.Fail(ex);
            throw new ServiceFailException(MessageResources.PatientSetFail);
        }
    }

    public IAsyncEnumerable<DoctorSchedule> GetAvailabilityAsync(string speciality, DateTime dateTime, 
        CancellationToken cancellationToken = default)
    {
        var date = DateOnly.FromDateTime(dateTime);
        return doctorRepository.FindBySpecialtyWithAvailabilityAsync(speciality, date, cancellationToken);
    }
}