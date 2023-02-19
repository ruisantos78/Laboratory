using RuiSantos.ZocDoc.Core.Models;

namespace RuiSantos.ZocDoc.Api.Contracts;

/// <summary>
/// Contract for a doctor's availability on a specific date
/// </summary>
public class DoctorAvailabilityContract
{
    /// <summary>
    /// The doctor license number
    /// </summary>
    public string License { get; set; }

    /// <summary>
    /// The doctor's first name
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    /// The doctor's last name
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    /// An array of medical specialties
    /// </summary>
    public IEnumerable<string> Specialties { get; set; }

    /// <summary>
    /// An array with the availability schedule for the doctor on a specific date
    /// </summary>
    public IEnumerable<DateTime> Schedule { get; set; }

    /// <summary>
    /// The doctor's email address
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// An array of contact numbers for the doctor
    /// </summary>
    public IEnumerable<string> ContactNumbers { get; set; }

    public DoctorAvailabilityContract() : this(new Doctor(), Enumerable.Empty<DateTime>()) { }

    public DoctorAvailabilityContract(Doctor model, IEnumerable<DateTime> schedule)
    {
        License = model.License;
        FirstName = model.FirstName;
        LastName = model.LastName;
        Specialties = model.Specialties;
        Schedule = schedule;
        Email = model.Email;
        ContactNumbers = model.ContactNumbers;
    }
}
