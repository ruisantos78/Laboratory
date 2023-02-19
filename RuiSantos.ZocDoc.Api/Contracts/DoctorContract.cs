using RuiSantos.ZocDoc.Core.Models;

namespace RuiSantos.ZocDoc.Api.Contracts;

/// <summary>
/// Contract for a doctor's information
/// </summary>
public class DoctorContract
{
    /// <summary>
    /// The doctor's license number.
    /// </summary>
    public string License { get; set; }

    /// <summary>
    /// The doctor's email.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// The doctor's first name.
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    /// The doctor's last name.
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    /// An array of contact numbers for the doctor.
    /// </summary>
    public IEnumerable<string> ContactNumbers { get; set; }

    /// <summary>
    /// An array of medical specialties that the doctor is trained in.
    /// </summary>
    public IEnumerable<string> Specialties { get; set; }

    public DoctorContract() : this(new Doctor()) { }

    public DoctorContract(Doctor model)
    {
        License = model.License;
        Email = model.Email;
        FirstName = model.FirstName;
        LastName = model.LastName;
        ContactNumbers = model.ContactNumbers;
        Specialties = model.Specialties;
    }
}
