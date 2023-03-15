using RuiSantos.ZocDoc.Core.Models;

namespace RuiSantos.ZocDoc.Core.Managers;

/// <summary>
/// Interface for managing doctors.
/// </summary>
public interface IDoctorManagement
{
    /// <summary>
    /// Creates a new doctor.
    /// </summary>
    /// <param name="license">The doctor's license number.</param>
    /// <param name="email">The doctor's email address.</param>
    /// <param name="firstName">The doctor's first name.</param>
    /// <param name="lastName">The doctor's last name.</param>
    /// <param name="contactNumbers">The doctor's contact numbers.</param>
    /// <param name="specialties">The doctor's specialties.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task CreateDoctorAsync(string license, string email, string firstName, string lastName, IEnumerable<string> contactNumbers, IEnumerable<string> specialties);
    
    IAsyncEnumerable<PatientAppointment> GetAppointmentsAsync(string license, DateTime? dateTime);

    Task<Doctor?> GetDoctorByLicenseAsync(string license);
    
    Task SetOfficeHoursAsync(string license, DayOfWeek dayOfWeek, IEnumerable<TimeSpan> hours);
}