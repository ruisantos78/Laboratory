using Microsoft.Extensions.Logging;
using RuiSantos.ZocDoc.Core.Adapters;
using RuiSantos.ZocDoc.Core.Managers.Exceptions;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Core.Resources;
using static RuiSantos.ZocDoc.Core.Resources.ManagementUtils;

namespace RuiSantos.ZocDoc.Core.Managers;

/// <summary>
/// Appointment management interface.
/// </summary>
public interface IAppointmentManagement
{
    /// <summary>
    /// Gets the appointments for a given patient.
    /// </summary>
    /// <param name="socialNumber">The social number of the patient.</param>
    /// <param name="medicalLicence">The medical licence of the doctor.</param>
    /// <param name="dateTime">The date and time of the appointment.</param>
    /// <exception cref="ValidationFailException">Thrown when the validation fails.</exception>
    /// <exception cref="ManagementFailException">Thrown when the appointment creation fails.</exception>
    Task CreateAppointmentAsync(string socialNumber, string medicalLicence, DateTime dateTime);

    /// <sumary>
    /// Deletes an appointment for a given patient.
    /// </sumary>
    /// <param name="socialNumber">The social number of the patient.</param>
    /// <param name="medicalLicence">The medical licence of the doctor.</param>
    /// <param name="dateTime">The date and time of the appointment.</param>
    /// <exception cref="ValidationFailException">Thrown when the validation fails.</exception>
    /// <exception cref="ManagementFailException">Thrown when the appointment deletion fails.</exception>
    Task DeleteAppointmentAsync(string socialNumber, string medicalLicence, DateTime dateTime);

    /// <summary>
    /// Gets the availability of a doctor.
    /// </summary>
    /// <param name="speciality">Speciality of the doctor.</param>
    /// <param name="dateTime">Date and time of the availability.</param>
    /// <returns>An asynchronous list of DoctorSchedule.</returns>
    IAsyncEnumerable<DoctorSchedule> GetAvailabilityAsync(string speciality, DateTime dateTime);
}

internal class AppointmentManagement : IAppointmentManagement
{
    private readonly IDoctorAdapter doctorAdapter;
    private readonly IPatientAdapter patientAdapter;
    private readonly ILogger logger;

    public AppointmentManagement(IDoctorAdapter doctorAdapter, IPatientAdapter patientAdapter, ILogger<AppointmentManagement> logger)
    {
        this.doctorAdapter = doctorAdapter;
        this.patientAdapter = patientAdapter;
        this.logger = logger;
    }

    public async Task CreateAppointmentAsync(string socialNumber, string medicalLicence, DateTime dateTime)
    {
        try
        {
            var patient = await patientAdapter.FindAsync(socialNumber);
            if (patient is null)
                throw new ValidationFailException(MessageResources.PatientSocialNumberNotFound);

            if (patient.Appointments.Any(appointment => appointment.GetDateTime().Equals(dateTime)))
                throw new ValidationFailException(MessageResources.RecordAlreadyExists);

            var doctor = await doctorAdapter.FindAsync(medicalLicence);
            if (doctor is null)
                throw new ValidationFailException(MessageResources.DoctorLicenseNotFound);

            if (doctor.Appointments.Any(appointment => appointment.GetDateTime().Equals(dateTime)))
                throw new ValidationFailException(MessageResources.RecordAlreadyExists);

            var appointment = new Appointment(dateTime);

            doctor.Appointments.Add(appointment);
            if (!IsValid(doctor, out var validationFailException))
                throw validationFailException;

            await doctorAdapter.StoreAsync(doctor);

            patient.Appointments.Add(appointment);
            if (!IsValid(patient, out validationFailException))
                throw validationFailException;

            await patientAdapter.StoreAsync(patient);
        }        
        catch (ValidationFailException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger?.Fail(ex);
            throw new ManagementFailException(MessageResources.PatientSetFail);
        }
    }

    public async Task DeleteAppointmentAsync(string socialNumber, string medicalLicence, DateTime dateTime)
    {
        try
        {
            var patient = await patientAdapter.FindAsync(socialNumber);
            if (patient is null)
                throw new ValidationFailException(MessageResources.PatientSocialNumberNotFound);

            var appointment = patient.Appointments.FirstOrDefault(app => app.GetDateTime() == dateTime);
            if (appointment is null)
                return;

            patient.Appointments.Remove(appointment);
            await patientAdapter.StoreAsync(patient);

            var doctor = await doctorAdapter.FindAsync(medicalLicence);
            if (doctor is null)
                return;

            doctor.Appointments.Remove(appointment);
            await doctorAdapter.StoreAsync(doctor);
        }         
        catch (ValidationFailException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger?.Fail(ex);
            throw new ManagementFailException(MessageResources.PatientSetFail);
        }
    }

    public async IAsyncEnumerable<DoctorSchedule> GetAvailabilityAsync(string speciality, DateTime dateTime)
    {
        var date = DateOnly.FromDateTime(dateTime);

        var doctors = await doctorAdapter.FindBySpecialtyWithAvailabilityAsync(speciality, date);
        foreach (var doctor in doctors)
        {
            var officeHours = doctor.OfficeHours.Where(hour => hour.Week == date.DayOfWeek)
                .SelectMany(hour => hour.Hours);

            var appointments = doctor.Appointments.Where(appointment => appointment.Date == date)
                .Select(appointment => appointment.Time);

            var schedule = officeHours.Except(appointments)
                .Select(time => date.WithTime(time));

            yield return new(doctor, schedule);
        }
    }
}