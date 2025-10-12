using RuiSantos.Labs.Core.Models;
using RuiSantos.Labs.Core.Services.Exceptions;

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
    /// <param name="cancellationToken"></param>
    /// <exception cref="ValidationFailException">Thrown when the doctor's license number is not unique.</exception>
    /// <exception cref="ServiceFailException">Thrown when the operation fails.</exception>
    Task CreateDoctorAsync(string license, string email, string firstName, string lastName,
        IEnumerable<string> contactNumbers, IEnumerable<string> specialties, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the doctor's appointments on a given date.
    /// </summary>
    /// <param name="doctorId">The doctor's identification.</param>
    /// <param name="dateTime">The date.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The doctor's appointments on the given date.</returns>
    Task<IEnumerable<PatientAppointment>> GetAppointmentsAsync(Guid doctorId, DateTime? dateTime,
        CancellationToken cancellationToken = default);


    Task<Pagination<Doctor>> GetAllDoctorsAsync(int take, string? paginationToken,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Set the office hours for a doctor.
    /// </summary>
    /// <param name="doctorId">The doctor's identification.</param>
    /// <param name="dayOfWeek">The day of the week.</param>
    /// <param name="hours">The office hours.</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ValidationFailException">Thrown when the doctor's license number is not found.</exception>
    /// <exception cref="ServiceFailException">Thrown when the operation fails.</exception>
    Task SetOfficeHoursAsync(Guid doctorId, DayOfWeek dayOfWeek, IEnumerable<TimeSpan> hours, 
        CancellationToken cancellationToken = default);

    Task<Doctor?> GetDoctorByLicenseAsync(string license, CancellationToken cancellationToken = default);
}