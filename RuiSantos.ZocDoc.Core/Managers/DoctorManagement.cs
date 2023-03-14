using Microsoft.Extensions.Logging;
using RuiSantos.ZocDoc.Core.Data;
using RuiSantos.ZocDoc.Core.Managers.Exceptions;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Core.Resources;
using static RuiSantos.ZocDoc.Core.Resources.ManagementUtils;

namespace RuiSantos.ZocDoc.Core.Managers;

/// <summary>
/// Manages the creation and modification of doctors.
/// </summary>
internal class DoctorManagement : IDoctorManagement
{
    private readonly IDomainContext domainContext;
    private readonly IDataContext context;
    private readonly ILogger logger;

    /// <summary>
    /// Creates a new instance of the DoctorManagement class.
    /// </summary>
    /// <param name="domainContext">The domain context.</param>
    /// <param name="context">The data context.</param>
    /// <param name="logger">The logger.</param>
    public DoctorManagement(IDomainContext domainContext, IDataContext context, ILogger<DoctorManagement> logger)
    {
        this.domainContext = domainContext;
        this.context = context;
        this.logger = logger;
    }

    /// <summary>
    /// Creates a new doctor.
    /// </summary>
    /// <param name="license">The doctor's license number.</param>
    /// <param name="email">The doctor's email.</param>
    /// <param name="firstName">The doctor's first name.</param>
    /// <param name="lastName">The doctor's last name.</param>
    /// <param name="contactNumbers">The doctor's contact numbers.</param>
    /// <param name="specialties">The doctor's specialties.</param>
    /// <exception cref="ValidationFailException">Thrown when the doctor's license number is not unique.</exception>
    /// <exception cref="ManagementFailException">Thrown when the operation fails.</exception>
    public async Task CreateDoctorAsync(string license, string email, string firstName, string lastName,
        IEnumerable<string> contactNumbers, IEnumerable<string> specialties)
    {
        try
        {
            var doctor = new Doctor(license, email, firstName, lastName, contactNumbers, specialties);
            await ValidateDoctorAsync(doctor);
            await context.StoreAsync(doctor);
        }
        catch (ValidationFailException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger?.Fail(ex);
            throw new ManagementFailException(MessageResources.DoctorSetFail);
        }
    }

    /// <summary>
    /// Set the office hours for a doctor.
    /// </summary>
    /// <param name="license">The doctor's license number.</param>
    /// <param name="dayOfWeek">The day of the week.</param>
    /// <param name="hours">The office hours.</param>
    /// <exception cref="ValidationFailException">Thrown when the doctor's license number is not found.</exception>
    /// <exception cref="ManagementFailException">Thrown when the operation fails.</exception>
    public async Task SetOfficeHoursAsync(string license, DayOfWeek dayOfWeek, IEnumerable<TimeSpan> hours)
    {
        try
        {
            var doctor = await context.FindAsync<Doctor>(i => i.License == license);
            if (doctor is null)
                throw new ValidationFailException(MessageResources.DoctorLicenseNotFound);

            doctor.OfficeHours.RemoveAll(hour => hour.Week == dayOfWeek);
            if (hours.Any())
                doctor.OfficeHours.Add(new OfficeHour(dayOfWeek, hours));

            await CancelAppointmentsAsync(doctor, dayOfWeek, hours);
            await context.StoreAsync(doctor);
        }
        catch (ValidationFailException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger?.Fail(ex);
            throw new ManagementFailException(MessageResources.DoctorSetFail);
        }
    }

    /// <summary>
    /// Get the doctor's informations by a given license number.
    /// </summary>
    /// <param name="license">The doctor's license number.</param>
    /// <returns>The doctor's informations.</returns>
    /// <exception cref="ManagementFailException">Thrown when the operation fails.</exception>
    public async Task<Doctor?> GetDoctorByLicenseAsync(string license)
    {
        try
        {
            return await context.FindAsync<Doctor>(doctor => doctor.License == license);
        }
        catch (Exception ex)
        {
            logger?.Fail(ex);
            throw new ManagementFailException(MessageResources.DoctorsGetFail);
        }
    }

    /// <summary>
    /// Get the doctor's appointments on a given date.
    /// </summary>
    /// <param name="license">The doctor's license number.</param>
    /// <param name="dateTime">The date.</param>
    /// <returns>The doctor's appointments on the given date.</returns> 
    public async IAsyncEnumerable<PatientAppointment> GetAppointmentsAsync(string license, DateTime? dateTime)
    {
        var date = DateOnly.FromDateTime(dateTime ?? DateTime.Today);

        var doctor = await context.FindAsync<Doctor>(doctor => doctor.License == license && doctor.Appointments.Any(da => da.Date == date));
        if (doctor is null)
            yield break;

        var appointments = doctor.Appointments.FindAll(a => a.Date == date);

        var patients = await context.QueryAsync<Patient>(p => p.Appointments.Intersect(appointments).Any());
        foreach (var patient in patients)
        {
            var patientAppointments = patient.Appointments.Where(pa => appointments.Any(da => da.Id == pa.Id));
            foreach (var pa in patientAppointments)
                yield return new(patient, pa.GetDateTime());
        }
    }

    /// <summary>
    /// Validates a doctor.
    /// </summary>
    /// <param name="model">The doctor.</param>
    /// <exception cref="ValidationFailException">Thrown if the doctor is invalid.</exception>
    private async Task ValidateDoctorAsync(Doctor model)
    {
        var medicalSpecialties = await domainContext.GetMedicalSpecialtiesAsync();
        if (!IsValid(model, out var validationFailException, medicalSpecialties))
            throw validationFailException;
    }

    /// <summary>
    /// Cancels appointments for a given doctor.
    /// </summary>
    /// <param name="doctor">The doctor.</param>
    /// <param name="dayOfWeek">The day of the week.</param>
    /// <param name="hours">The office hours.</param>    
    private async Task CancelAppointmentsAsync(Doctor doctor, DayOfWeek dayOfWeek, IEnumerable<TimeSpan> hours)
    {
        var appointments = doctor.Appointments.FindAll(appointment => appointment.Week == dayOfWeek && !hours.Contains(appointment.Time));
        if (appointments is null || !appointments.Any())
            return;

        var patients = await context.QueryAsync<Patient>(patient => patient.Appointments.Any(item => appointments.Any(a => a.Id == item.Id)));
        if (patients is not null && patients.Any())
        {
            patients.ForEach(async p =>
            {
                p.Appointments.RemoveAll(pa => appointments.Any(da => da.Id == pa.Id));
                await context.StoreAsync(p);
            });
        }

        doctor.Appointments.RemoveAll(item => appointments.Any(a => a.Id == item.Id));
    }
}
