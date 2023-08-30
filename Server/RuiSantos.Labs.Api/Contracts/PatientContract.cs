using RuiSantos.Labs.Core.Models;

namespace RuiSantos.Labs.Api.Contracts;

/// <summary>
/// Contract for a patient's information
/// </summary>
public class PatientContract
{
    public static readonly PatientContract Empty = new();

    /// <summary>
    /// The Social Security Number of the patient
    /// </summary>
    public string SocialSecurityNumber { get; init; }

    /// <summary>
    /// The email of the patient
    /// </summary>
    public string Email { get; init; }

    /// <summary>
    /// The first name of the patient
    /// </summary>
    public string FirstName { get; init; }

    /// <summary>
    /// The last name of the patient
    /// </summary>
    public string LastName { get; init; }

    /// <summary>
    /// An array of contact numbers of the patient
    /// </summary>  
    public IEnumerable<string> ContactNumbers { get; init; }

    public PatientContract() : this(Patient.Empty) { }

    public PatientContract(Patient model)
    {
        SocialSecurityNumber = model.SocialSecurityNumber;
        Email = model.Email;
        FirstName = model.FirstName;
        LastName = model.LastName;
        ContactNumbers = model.ContactNumbers;
    }
}
