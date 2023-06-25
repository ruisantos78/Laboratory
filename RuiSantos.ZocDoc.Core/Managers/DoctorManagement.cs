using Microsoft.Extensions.Logging;
using RuiSantos.ZocDoc.Core.Adapters;
using RuiSantos.ZocDoc.Core.Data;
using RuiSantos.ZocDoc.Core.Managers.Exceptions;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Core.Resources;
using static RuiSantos.ZocDoc.Core.Resources.ManagementUtils;

namespace RuiSantos.ZocDoc.Core.Managers;

/// <summary>
/// Manages the creation and modification of doctors.
/// </summary>
public interface IDoctorManagement
{
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
    Task CreateDoctorAsync(string license, string email, string firstName, string lastName, IEnumerable<string> contactNumbers, IEnumerable<string> specialties);

    /// <summary>
    /// Get the doctor's appointments on a given date.
    /// </summary>
    /// <param name="license">The doctor's license number.</param>
    /// <param name="dateTime">The date.</param>
    /// <returns>The doctor's appointments on the given date.</returns>
    IAsyncEnumerable<PatientAppointment> GetAppointmentsAsync(string license, DateTime? dateTime);

    /// <summary>
    /// Get the doctor's informations by a given license number.
    /// </summary>
    /// <param name="license">The doctor's license number.</param>
    /// <returns>The doctor's informations.</returns>
    /// <exception cref="ManagementFailException">Thrown when the operation fails.</exception>
    Task<Doctor?> GetDoctorByLicenseAsync(string license);

    /// <summary>
    /// Set the office hours for a doctor.
    /// </summary>
    /// <param name="license">The doctor's license number.</param>
    /// <param name="dayOfWeek">The day of the week.</param>
    /// <param name="hours">The office hours.</param>
    /// <exception cref="ValidationFailException">Thrown when the doctor's license number is not found.</exception>
    /// <exception cref="ManagementFailException">Thrown when the operation fails.</exception>
    Task SetOfficeHoursAsync(string license, DayOfWeek dayOfWeek, IEnumerable<TimeSpan> hours);
}

internal class DoctorManagement : IDoctorManagement
{
    private readonly IDomainContext domainContext;
    private readonly IDoctorAdapter doctorAdapter;
    private readonly IPatientAdapter patientAdapter;
    private readonly ILogger logger;

    public DoctorManagement(IDomainContext domainContext,
                            IDoctorAdapter doctorAdapter,
                            IPatientAdapter patientAdapter,
                            ILogger<DoctorManagement> logger)
    {
        this.domainContext = domainContext;
        this.doctorAdapter = doctorAdapter;
        this.patientAdapter = patientAdapter;
        this.logger = logger;
    }

    public async Task CreateDoctorAsync(string license, string email, string firstName, string lastName,
        IEnumerable<string> contactNumbers, IEnumerable<string> specialties)
    {
        try
        {
            var doctor = new Doctor() {
                License = license,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                ContactNumbers = contactNumbers.ToHashSet(),
                Specialties = specialties.ToHashSet()
            };

            await ValidateDoctorAsync(doctor);
            await doctorAdapter.StoreAsync(doctor);
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

    public async Task SetOfficeHoursAsync(string license, DayOfWeek dayOfWeek, IEnumerable<TimeSpan> hours)
    {
        try
        {
            var doctor = await doctorAdapter.FindAsync(license);
            if (doctor is null)
                throw new ValidationFailException(MessageResources.DoctorLicenseNotFound);

            doctor.OfficeHours.RemoveWhere(hour => hour.Week == dayOfWeek);
            if (hours.Any())
                doctor.OfficeHours.Add(new OfficeHour(dayOfWeek, hours));

            await CancelAppointmentsAsync(doctor, dayOfWeek, hours);
            await doctorAdapter.StoreAsync(doctor);
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

    public async Task<Doctor?> GetDoctorByLicenseAsync(string license)
    {
        try
        {
            return await doctorAdapter.FindAsync(license);
        }
        catch (Exception ex)
        {
            logger?.Fail(ex);
            throw new ManagementFailException(MessageResources.DoctorsGetFail);
        }
    }

    public async IAsyncEnumerable<PatientAppointment> GetAppointmentsAsync(string license, DateTime? dateTime)
    {
        var date = DateOnly.FromDateTime(dateTime ?? DateTime.Today);

        var doctor = await doctorAdapter.FindAsync(license);
        if (doctor is null)
            yield break;

        var appointments = doctor.Appointments.Where(a => a.Date == date).ToHashSet();
        if (!appointments.Any())
            yield break;

        var patients = await patientAdapter.FindAllWithAppointmentsAsync(doctor.Appointments);
        foreach (var patient in patients)
        {
            var patientAppointments = patient.Appointments.Where(pa => appointments.Any(da => da.Id == pa.Id));
            foreach (var pa in patientAppointments)
                yield return new(patient, pa.GetDateTime());
        }
    }

    private async Task ValidateDoctorAsync(Doctor model)
    {
        var medicalSpecialties = await domainContext.GetMedicalSpecialtiesAsync();
        if (!IsValid(model, out var validationFailException, medicalSpecialties))
            throw validationFailException;
    }

    private async Task CancelAppointmentsAsync(Doctor doctor, DayOfWeek dayOfWeek, IEnumerable<TimeSpan> hours)
    {
        var appointments = doctor.Appointments
            .Where(appointment => appointment.Week == dayOfWeek && !hours.Contains(appointment.Time))
            .ToHashSet();

        if (!appointments.Any())
            return;

        var patients = await patientAdapter.FindAllWithAppointmentsAsync(appointments);
        if (patients.Any())
        {
            await Task.WhenAll(patients.Select(p =>
            {
                p.Appointments.RemoveWhere(pa => appointments.Any(da => da.Id == pa.Id));
                return patientAdapter.StoreAsync(p);
            }).ToArray());
        }

        doctor.Appointments.RemoveWhere(item => appointments.Any(a => a.Id == item.Id));
    }
}
