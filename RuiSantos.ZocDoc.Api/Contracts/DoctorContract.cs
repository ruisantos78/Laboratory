using RuiSantos.ZocDoc.Core.Models;

namespace RuiSantos.ZocDoc.Api.Contracts;

/// <summary>
/// Doctor's informations
/// </summary>
public class DoctorContract
{
    /// <summary>
    /// Doctor license number
    /// </summary>
    public string License { get; set; }

    /// <summary>
    /// E-mail
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// First name
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    /// Last name
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    /// Array of contact numbers
    /// </summary>
    public IEnumerable<string> ContactNumbers { get; set; }

    /// <summary>
    /// Array of medical spcialties
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
