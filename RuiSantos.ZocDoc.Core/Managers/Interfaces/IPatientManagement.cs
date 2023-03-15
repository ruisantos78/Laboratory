using RuiSantos.ZocDoc.Core.Models;

namespace RuiSantos.ZocDoc.Core.Managers;

/// <summary>
/// Interface for managing patients.
/// </summary>
public interface IPatientManagement
{
    /// <summary>
    /// Creates a new patient.
    /// </summary>
    /// <param name="socialNumber">Social number of the patient.</param>
    /// <param name="email">Email of the patient.</param>
    /// <param name="firstName">First name of the patient.</param>
    /// <param name="lastName">Last name of the patient.</param>
    /// <param name="contactNumbers">Contact numbers of the patient.</param>
    Task CreatePatientAsync(string socialNumber, string email, string firstName, string lastName, IEnumerable<string> contactNumbers);

    /// <summary>
    /// Gets all the appointments of a patient.
    /// </summary>
    /// <param name="socialNumber">Social number of the patient.</param>
    /// <returns>List of appointments.</returns>
    IAsyncEnumerable<DoctorAppointment> GetAppointmentsAsync(string socialNumber);

    /// <summary>
    /// Gets a patient by social number.
    /// </summary>
    /// <param name="socialNumber">Social number of the patient.</param>
    /// <returns>Patient.</returns>
    Task<Patient?> GetPatientBySocialNumberAsync(string socialNumber);
}