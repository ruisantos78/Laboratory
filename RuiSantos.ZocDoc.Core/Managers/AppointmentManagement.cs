using Microsoft.Extensions.Logging;
using RuiSantos.ZocDoc.Core.Data;
using RuiSantos.ZocDoc.Core.Managers.Exceptions;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Core.Resources;
using static RuiSantos.ZocDoc.Core.Resources.ManagementUtils;

namespace RuiSantos.ZocDoc.Core.Managers;

/// <summary>
/// Manages the creation and deletion of appointments.
/// </summary>
public class AppointmentManagement
{
    private readonly IDataContext context;
    private readonly ILogger logger;

    /// <summary>
    /// Creates a new instance of <see cref="AppointmentManagement"/>.
    /// </summary>
    /// <param name="context">The <see cref="IDataContext"/> to use.</param>
    /// <param name="logger">The <see cref="ILogger"/> to use.</param>
    public AppointmentManagement(IDataContext context, ILogger<AppointmentManagement> logger)
    {
        this.context = context;
        this.logger = logger;
    }

    /// <summary>
    /// Gets the appointments for a given patient.
    /// </summary>
    /// <param name="socialNumber">The social number of the patient.</param>
    /// <param name="medicalLicence">The medical licence of the doctor.</param>
    /// <param name="dateTime">The date and time of the appointment.</param>
    /// <exception cref="ValidationFailException">Thrown when the validation fails.</exception>
    /// <exception cref="ManagementFailException">Thrown when the appointment creation fails.</exception>
    public async Task CreateAppointmentAsync(string socialNumber, string medicalLicence, DateTime dateTime)
    {
        try
        {
            var patient = await context.FindAsync<Patient>(patient => patient.SocialSecurityNumber == socialNumber);
            if (patient is null)
                throw new ValidationFailException(MessageResources.PatientSocialNumberNotFound);

            if (patient.Appointments.Any(appointment => appointment.GetDateTime().Equals(dateTime)))
                throw new ValidationFailException(MessageResources.RecordAlreadyExists);

            var doctor = await context.FindAsync<Doctor>(doctor => doctor.License == medicalLicence);
            if (doctor is null)
                throw new ValidationFailException(MessageResources.DoctorLicenseNotFound);

            if (doctor.Appointments.Any(appointment => appointment.GetDateTime().Equals(dateTime)))
                throw new ValidationFailException(MessageResources.RecordAlreadyExists);

            var appointment = new Appointment(dateTime);

            doctor.Appointments.Add(appointment);
            if (!IsValid(doctor, out var validationFailException))
                throw validationFailException;

            await context.StoreAsync(doctor);

            patient.Appointments.Add(appointment);
            if (!IsValid(patient, out validationFailException))
                throw validationFailException;

            await context.StoreAsync(patient);
        }
        catch (ValidationFailException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger?.LogException(ex);
            throw new ManagementFailException(MessageResources.PatientSetFail);
        }
    }

    /// <sumary>
    /// Deletes an appointment for a given patient.
    /// </sumary>
    /// <param name="socialNumber">The social number of the patient.</param>
    /// <param name="medicalLicence">The medical licence of the doctor.</param>
    /// <param name="dateTime">The date and time of the appointment.</param>
    /// <exception cref="ValidationFailException">Thrown when the validation fails.</exception>
    /// <exception cref="ManagementFailException">Thrown when the appointment deletion fails.</exception>
    public async Task DeleteAppointmentAsync(string socialNumber, string medicalLicence, DateTime dateTime)
    {
        try
        {
            var patient = await context.FindAsync<Patient>(patient => patient.SocialSecurityNumber == socialNumber);
            if (patient is null)
                throw new ValidationFailException(MessageResources.PatientSocialNumberNotFound);

            var appointment = patient.Appointments.FirstOrDefault(app => app.GetDateTime() == dateTime);
            if (appointment is null)
                return;

            patient.Appointments.Remove(appointment);
            await context.StoreAsync(patient);

            var doctor = await context.FindAsync<Doctor>(doctor => doctor.License == medicalLicence);
            if (doctor is null)
                return;

            if (doctor.Appointments.RemoveAll(app => app.Id == appointment.Id) > 0)
                await context.StoreAsync(doctor);
        }
        catch (ValidationFailException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger?.LogException(ex);
            throw new ManagementFailException(MessageResources.PatientSetFail);
        }
    }

    /// <summary>
    /// Gets the availability for doctors with a specialty on a given date.
    /// </summary>
    /// <param name="speciality">The specialty.</param>
    /// <param name="dateTime">The date and time of the appointment.</param>   
    /// <returns>The availability for doctors with a specialty on a given date</returns> 
    public async IAsyncEnumerable<(Doctor doctor, IEnumerable<DateTime> schedule)> GetAvailabilityAsync(string speciality, DateTime dateTime)
    {
        var date = DateOnly.FromDateTime(dateTime);

        var doctors = await GetDoctorsBySpecialtyWithAvailabilityAsync(speciality, date);
        foreach (var doctor in doctors)
        {
            var officeHours = doctor.OfficeHours.Where(hour => hour.Week == date.DayOfWeek)
                .SelectMany(hour => hour.Hours);

            var appointments = doctor.Appointments.Where(appointment => appointment.Date == date)
                .Select(appointment => appointment.Time);

            var schedule = officeHours.Except(appointments)
                .Select(time => date.ToDateTime(time));

            yield return (doctor, schedule);
        }
    }

    /// <summary>
    /// Gets the doctors with a specialty and a free agenda on a given date
    /// </summary>
    /// <param name="speciality">The specialty.</param>
    /// <param name="date">The date and time of the appointment.</param>
    /// <exception cref="ManagementFailException">Thrown when the doctors retrieval fails.</exception>
    /// <returns>The doctors with a specialty and a free agenda on a given date</returns> 
    private async Task<IEnumerable<Doctor>> GetDoctorsBySpecialtyWithAvailabilityAsync(string speciality, DateOnly date)
    {
        try
        {
            return await context.QueryAsync<Doctor>(dr =>
                dr.Specialties.Any(ds => ds.Equals(speciality, StringComparison.OrdinalIgnoreCase)) &&
                !dr.OfficeHours.All(oh =>
                    oh.Week == date.DayOfWeek &&
                    oh.Hours.Any(hour => dr.Appointments.Any(app => app.Date == date && app.Time == hour))
                )
            );
        }
        catch (Exception ex)
        {
            logger?.LogException(ex);
            throw new ManagementFailException(MessageResources.DoctorsGetFail);
        }
    }
}
