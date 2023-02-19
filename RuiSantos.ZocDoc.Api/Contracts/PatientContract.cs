using RuiSantos.ZocDoc.Core.Models;

namespace RuiSantos.ZocDoc.Api.Contracts;

/// <summary>
/// Contract for a patient's information
/// </summary>
public class PatientContract
{
    /// <summary>
    /// The Social Security Number of the patient
    /// </summary>
    public string SocialSecurityNumber { get; set; }

    /// <summary>
    /// The email of the patient
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// The first name of the patient
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    /// The last name of the patient
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    /// An array of contact numbers of the patient
    /// </summary>
    public IEnumerable<string> ContactNumbers { get; set; }

    public PatientContract() : this(new Patient()) { }

    public PatientContract(Patient model)
    {
        SocialSecurityNumber = model.SocialSecurityNumber;
        Email = model.Email;
        FirstName = model.FirstName;
        LastName = model.LastName;
        ContactNumbers = model.ContactNumbers;
    }
}
