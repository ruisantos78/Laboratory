using RuiSantos.Labs.Core.Services.Exceptions;
using RuiSantos.Labs.Domain.Models;

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
    /// <param name="cancellationToken"></param>
    /// <exception cref="ValidationFailException">Thrown when the validation fails.</exception>
    Task CreateAppointmentAsync(string socialNumber, Guid doctorId, DateTime dateTime,
        CancellationToken cancellationToken = default);

    /// <sumary>
    /// Deletes an appointment for a given patient.
    /// </sumary>
    /// <param name="socialNumber">The social number of the patient.</param>
    /// <param name="doctorId">The doctor's identification.</param>
    /// <param name="dateTime">The date and time of the appointment.</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ValidationFailException">Thrown when the validation fails.</exception>
    Task DeleteAppointmentAsync(string socialNumber, Guid doctorId, DateTime dateTime,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the availability of a doctor.
    /// </summary>
    /// <param name="speciality">Speciality of the doctor.</param>
    /// <param name="dateTime">Date and time of the availability.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>An asynchronous list of DoctorSchedule.</returns>
    IAsyncEnumerable<DoctorSchedule> GetAvailabilityAsync(string speciality, DateTime dateTime,
        CancellationToken cancellationToken = default);
}