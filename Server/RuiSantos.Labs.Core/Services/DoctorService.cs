using Microsoft.Extensions.Logging;
using RuiSantos.Labs.Core.Models;
using RuiSantos.Labs.Core.Repositories;
using RuiSantos.Labs.Core.Resources;
using RuiSantos.Labs.Core.Services.Exceptions;
using RuiSantos.Labs.Core.Validators;

namespace RuiSantos.Labs.Core.Services;

/// <summary>
/// Manages the creation and modification of doctors.
/// </summary>
public interface IDoctorService
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
    /// <exception cref="ServiceFailException">Thrown when the operation fails.</exception>
    Task CreateDoctorAsync(string license, string email, string firstName, string lastName, IEnumerable<string> contactNumbers, IEnumerable<string> specialties);

    /// <summary>
    /// Get the doctor's appointments on a given date.
    /// </summary>
    /// <param name="doctorId">The doctor's identification.</param>
    /// <param name="dateTime">The date.</param>
    /// <returns>The doctor's appointments on the given date.</returns>
    Task<IEnumerable<PatientAppointment>> GetAppointmentsAsync(Guid doctorId, DateTime? dateTime);

    /// <summary>
    /// Get the doctor's informations.
    /// </summary>
    /// <param name="doctorId">The doctor's identification.</param>
    /// <returns>The doctor's informations.</returns>
    /// <exception cref="ServiceFailException">Thrown when the operation fails.</exception>
    Task<Doctor?> GetDoctorAsync(Guid doctorId);

    
    Task<IEnumerable<Doctor>> GetAllDoctors(int take, string? from = null);

    /// <summary>
    /// Set the office hours for a doctor.
    /// </summary>
    //// <param name="id">The doctor's identification.</param>
    /// <param name="dayOfWeek">The day of the week.</param>
    /// <param name="hours">The office hours.</param>
    /// <exception cref="ValidationFailException">Thrown when the doctor's license number is not found.</exception>
    /// <exception cref="ServiceFailException">Thrown when the operation fails.</exception>
    Task SetOfficeHoursAsync(Guid doctorId, DayOfWeek dayOfWeek, IEnumerable<TimeSpan> hours);

    Task<Doctor?> GetDoctorByLicenseAsync(string license);
}

internal class DoctorService : IDoctorService
{
    private readonly IDoctorRepository doctorRepository;
    private readonly IAppointamentsRepository appointamentsRepository;
    private readonly ILogger logger;

    public DoctorService(
        IDoctorRepository doctorRepository,
        IAppointamentsRepository appointamentsRepository,
        ILogger<DoctorService> logger)
    {
        this.doctorRepository = doctorRepository;
        this.appointamentsRepository = appointamentsRepository;
        this.logger = logger;
    }

    public async Task CreateDoctorAsync(string license, string email, string firstName, string lastName,
        IEnumerable<string> contactNumbers, IEnumerable<string> specialties)
    {
        try
        {
            var doctor = new Doctor()
            {
                License = license,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                ContactNumbers = contactNumbers.ToHashSet(),
                Specialties = specialties.ToHashSet()
            };

            Validator.ThrowExceptionIfIsNotValid(doctor);
            await doctorRepository.StoreAsync(doctor);
        }
        catch (ValidationFailException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger?.Fail(ex);
            throw new ServiceFailException(MessageResources.DoctorSetFail);
        }
    }

    public async Task SetOfficeHoursAsync(Guid doctorId, DayOfWeek dayOfWeek, IEnumerable<TimeSpan> hours)
    {
        try
        {
            var doctor = await doctorRepository.FindAsync(doctorId) ??
                throw new ValidationFailException(MessageResources.DoctorLicenseNotFound);

            doctor.OfficeHours.RemoveWhere(hour => hour.Week == dayOfWeek);
            if (hours.Any())
                doctor.OfficeHours.Add(new OfficeHour(dayOfWeek, hours));

            await doctorRepository.StoreAsync(doctor);
        }
        catch (ValidationFailException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger?.Fail(ex);
            throw new ServiceFailException(MessageResources.DoctorSetFail);
        }
    }

    public async Task<Doctor?> GetDoctorAsync(Guid doctorId)
    {
        try
        {
            return await doctorRepository.FindAsync(doctorId);
        }
        catch (Exception ex)
        {
            logger?.Fail(ex);
            throw new ServiceFailException(MessageResources.DoctorsGetFail);
        }
    }

    public async Task<IEnumerable<PatientAppointment>> GetAppointmentsAsync(Guid doctorId, DateTime? dateTime)
    {
        try {
            var date = DateOnly.FromDateTime(dateTime ?? DateTime.Today);

            var doctor = await doctorRepository.FindAsync(doctorId);
            if (doctor is null)
                return Array.Empty<PatientAppointment>();

            var result = await appointamentsRepository.GetPatientAppointmentsAsync(doctor, date)
                .ToArrayAsync();

            return result ?? Array.Empty<PatientAppointment>();
        } 
        catch(Exception ex) 
        {
            logger?.Fail(ex);
            throw new ServiceFailException(MessageResources.DoctorsGetAppointmentsFail);
        }
    }

    public async Task<IEnumerable<Doctor>> GetAllDoctors(int take, string? last = null)
    {
        try
        {
            var result = await doctorRepository.FindAllAsync(take, last)
                .ToArrayAsync();

            return result ?? Array.Empty<Doctor>();
        }
        catch (Exception ex)
        {
            logger?.Fail(ex);
            throw new ServiceFailException(MessageResources.DoctorsGetAppointmentsFail);
        }
    }

    public async Task<Doctor?> GetDoctorByLicenseAsync(string license)
    {
        try
        {
            return await doctorRepository.FindByLicenseAsync(license);
        }
        catch (Exception ex)
        {
            logger?.Fail(ex);
            throw new ServiceFailException(MessageResources.DoctorsGetFail);
        }
    }
}
