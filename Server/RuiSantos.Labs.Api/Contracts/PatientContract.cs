using RuiSantos.Labs.Core.Models;

namespace RuiSantos.Labs.Api.Contracts;

/// <summary>
/// Contract for a patient's information
/// </summary>
public class PatientContract(Patient model)
{

    /// <summary>
    /// The Social Security Number of the patient
    /// </summary>
    public string SocialSecurityNumber => model.SocialSecurityNumber;

    /// <summary>
    /// The email of the patient
    /// </summary>
    public string Email => model.Email;

    /// <summary>
    /// The first name of the patient
    /// </summary>
    public string FirstName => model.FirstName;

    /// <summary>
    /// The last name of the patient
    /// </summary>
    public string LastName => model.LastName;

    /// <summary>
    /// An array of contact numbers of the patient
    /// </summary>  
    public IEnumerable<string> ContactNumbers => model.ContactNumbers;

    public PatientContract() : this(Patient.Empty)
    {
    }

    public static implicit operator PatientContract(Patient model) => new(model);
}
