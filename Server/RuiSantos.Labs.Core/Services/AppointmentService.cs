using Microsoft.Extensions.Logging;
using RuiSantos.Labs.Core.Extensions;
using RuiSantos.Labs.Core.Models;
using RuiSantos.Labs.Core.Repositories;
using RuiSantos.Labs.Core.Resources;
using RuiSantos.Labs.Core.Services.Exceptions;

namespace RuiSantos.Labs.Core.Services;

/// <summary>
/// Appointment management interface.
/// </summary>
public interface IAppointmentService
{
    /// <summary>
    /// Gets the appointments for a given patient.
    /// </summary>
    /// <param name="socialNumber">The social number of the patient.</param>
    /// <param name="doctorId">The doctor's identification.</param>
    /// <param name="dateTime">The date and time of the appointment.</param>
    /// <exception cref="ValidationFailException">Thrown when the validation fails.</exception>
    Task CreateAppointmentAsync(string socialNumber, Guid doctorId, DateTime dateTime);

    /// <sumary>
    /// Deletes an appointment for a given patient.
    /// </sumary>
    /// <param name="socialNumber">The social number of the patient.</param>
    /// <param name="doctorId">The doctor's identification.</param>
    /// <param name="dateTime">The date and time of the appointment.</param>
    /// <exception cref="ValidationFailException">Thrown when the validation fails.</exception>
    Task DeleteAppointmentAsync(string socialNumber, Guid doctorId, DateTime dateTime);

    /// <summary>
    /// Gets the availability of a doctor.
    /// </summary>
    /// <param name="speciality">Speciality of the doctor.</param>
    /// <param name="dateTime">Date and time of the availability.</param>
    /// <returns>An asynchronous list of DoctorSchedule.</returns>
    IAsyncEnumerable<DoctorSchedule> GetAvailabilityAsync(string speciality, DateTime dateTime);
}

internal class AppointmentService : IAppointmentService
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IAppointamentsRepository _appointamentsRepository;
    private readonly ILogger _logger;

    public AppointmentService(IDoctorRepository doctorRepository,
        IPatientRepository patientRepository,
        IAppointamentsRepository appointamentsRepository,
        ILogger<AppointmentService> logger)
    {
        _doctorRepository = doctorRepository;
        _patientRepository = patientRepository;
        _appointamentsRepository = appointamentsRepository;
        _logger = logger;
    }

    public async Task CreateAppointmentAsync(string socialNumber, Guid doctorId, DateTime dateTime)
    {
        try
        {
            if (await _doctorRepository.FindAsync(doctorId) is not { } doctor)
                throw new ValidationFailException(MessageResources.DoctorLicenseNotFound);

            if (await _appointamentsRepository.GetAsync(doctor, dateTime) is not null)
                throw new ValidationFailException(MessageResources.RecordAlreadyExists);

            if (await _patientRepository.FindAsync(socialNumber) is not { } patient)
                throw new ValidationFailException(MessageResources.PatientSocialNumberNotFound);

            if (await _appointamentsRepository.GetAsync(patient, dateTime) is not null)
                throw new ValidationFailException(MessageResources.RecordAlreadyExists);

            await _appointamentsRepository.StoreAsync(doctor, patient, dateTime);
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

    public async Task DeleteAppointmentAsync(string socialNumber, Guid doctorId, DateTime dateTime)
    {
        try
        {
            if (await _patientRepository.FindAsync(socialNumber) is not { } patient)
                throw new ValidationFailException(MessageResources.PatientSocialNumberNotFound);

            if (await _appointamentsRepository.GetAsync(patient, dateTime) is not { } patientAppointment)
                return;

            if (await _doctorRepository.FindAsync(doctorId) is not { } doctor)
                return;

            if (await _appointamentsRepository.GetAsync(doctor, dateTime) is not { } doctorAppointment)
                return;

            if (patientAppointment.Id != doctorAppointment.Id)
                return;

            await _appointamentsRepository.RemoveAsync(patientAppointment);
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

    public IAsyncEnumerable<DoctorSchedule> GetAvailabilityAsync(string speciality, DateTime dateTime)
    {
        var date = DateOnly.FromDateTime(dateTime);
        return _doctorRepository.FindBySpecialtyWithAvailabilityAsync(speciality, date);
    }
}