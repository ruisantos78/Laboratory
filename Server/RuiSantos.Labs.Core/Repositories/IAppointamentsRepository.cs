using RuiSantos.Labs.Core.Models;

namespace RuiSantos.Labs.Core.Repositories;

/// <summary>
/// Interface for the appointment repository.
/// </summary>
public interface IAppointamentsRepository
{
    /// <summary>
    /// Asynchronously gets information about appointments. This method is useful for polling for appointments that have been created in the past or for which you would like to know what date and time they were created.
    /// </summary>
    /// <param name="patient">The patient to retrieve information for. Must not be null.</param>
    /// <param name="dateTime">The date and time to retrieve information for.</param>
    /// <returns>An instance of representing the asynchronous operation</returns>
    Task<Appointment?> GetAsync(Patient patient, DateTime dateTime);
    
    /// <summary>
    /// Retrieves appointment data for a doctor. This will return an empty object if none exists.
    /// </summary>
    /// <param name="doctor">The doctor to retrieve data for.</param>
    /// <param name="dateTime">The date / time to retrieve data for.</param>
    /// <returns>The appointment data for the doctor or null if none exists in the system for the specified date / time</returns>
    Task<Appointment?> GetAsync(Doctor doctor, DateTime dateTime);

    /// <summary>
    /// Gets a list of appointments for a doctor. This will be empty if there are no appointments for the doctor.
    /// </summary>
    /// <param name="doctor">The doctor for whom to get appointments.</param>
    /// <param name="date">Specifies which date to get appointments for.</param>
    /// <returns>An enumeration of appointments for the doctor or an empty enumeration if there are no appointments</returns>
    Task<IEnumerable<PatientAppointment>> GetPatientAppointmentsAsync(Doctor doctor, DateOnly date);

    /// <summary>
    /// Asynchronously removes an appointment from the user's calendar. This is equivalent to a request to remove an appointment from the user's calendar.
    /// </summary>
    /// <param name="appointment">The appointment to remove. Must not be null.</param>
    /// <returns>A that represents the asynchronous operation. The task result contains the removed appointment in the User's calendar</returns>
    Task RemoveAsync(Appointment appointment);

    /// <summary>
    /// Stores a doctor in the database. This is a no - op if the doctor already exists
    /// </summary>
    /// <param name="doctor">The doctor to store.</param>
    /// <param name="patient">The patient to store. Must not be null.</param>
    /// <param name="dateTime">The date and time to store. May be null in which case the store will be created at the current time.</param>
    /// <returns>A task representing the asynchronous operation. When the task completes the property will contain the store information for the doctor</returns>
    Task StoreAsync(Doctor doctor, Patient patient, DateTime dateTime);
}
