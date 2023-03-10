using RuiSantos.ZocDoc.Core.Models;

namespace RuiSantos.ZocDoc.Core.Managers;

/// <summary>
/// Appointment management interface.
/// </summary>
public interface IAppointmentManagement
{
    /// <summary>
    /// Creates an appointment.
    /// </summary>
    /// <param name="socialNumber">Social number of the patient.</param>
    /// <param name="medicalLicence">Medical licence of the doctor.</param>
    /// <param name="dateTime">Date and time of the appointment.</param>
    /// <returns>An asynchronous task.</returns>
    Task CreateAppointmentAsync(string socialNumber, string medicalLicence, DateTime dateTime);

    /// <summary>
    /// Deletes an appointment.
    /// </summary>
    /// <param name="socialNumber">Social number of the patient.</param>
    /// <param name="medicalLicence">Medical licence of the doctor.</param>
    /// <param name="dateTime">Date and time of the appointment.</param>
    /// <returns>An asynchronous task.</returns>
    Task DeleteAppointmentAsync(string socialNumber, string medicalLicence, DateTime dateTime);

    /// <summary>
    /// Gets the availability of a doctor.
    /// </summary>
    /// <param name="speciality">Speciality of the doctor.</param>
    /// <param name="dateTime">Date and time of the availability.</param>
    /// <returns>An asynchronous list of DoctorSchedule.</returns>
    IAsyncEnumerable<DoctorSchedule> GetAvailabilityAsync(string speciality, DateTime dateTime);
}
