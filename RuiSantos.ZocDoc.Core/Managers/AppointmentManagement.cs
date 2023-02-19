using Microsoft.Extensions.Logging;
using RuiSantos.ZocDoc.Core.Data;
using RuiSantos.ZocDoc.Core.Managers.Exceptions;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Core.Resources;
using RuiSantos.ZocDoc.Core.Validators;

namespace RuiSantos.ZocDoc.Core.Managers;

public class AppointmentManagement : ManagementBase
{
    private readonly IDataContext context;
    private readonly ILogger logger;

    public AppointmentManagement(IDataContext context, ILogger<AppointmentManagement> logger)
    {
        this.context = context;
        this.logger = logger;
    }

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
            if (!IsValid(doctor, DoctorValidator.Instance, out var validationFailException))
                throw validationFailException;

            await context.StoreAsync(doctor);

            patient.Appointments.Add(appointment);
            if (!IsValid(patient, PatientValidator.Instance, out validationFailException))
                throw validationFailException;

            await context.StoreAsync(patient);
        }
        catch (ValidationFailException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger?.LogException(nameof(AppointmentManagement), nameof(CreateAppointmentAsync), ex);
            throw new ManagementFailException(MessageResources.PatientSetFail);
        }
    }

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
        catch (Exception ex)
        {
            logger?.LogException(nameof(AppointmentManagement), nameof(DeleteAppointmentAsync), ex);
            throw new ManagementFailException(MessageResources.PatientSetFail);
        }
    }

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
            logger?.LogException(nameof(AppointmentManagement), nameof(GetDoctorsBySpecialtyWithAvailabilityAsync), ex);
            throw new ManagementFailException(MessageResources.DoctorsGetFail);
        }
    }
}
