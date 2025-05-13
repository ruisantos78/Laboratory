using RuiSantos.Labs.Core.Models;

namespace RuiSantos.Labs.Api.Contracts;

/// <summary>
/// Contract for a doctor's information
/// </summary>
public class DoctorContract(Doctor model)
{
    /// <summary>
    /// The doctor's license number.
    /// </summary>
    public string License => model.License;

    /// <summary>
    /// The doctor's email.
    /// </summary>
    public string Email => model.Email;

    /// <summary>
    /// The doctor's first name.
    /// </summary>
    public string FirstName => model.FirstName;

    /// <summary>
    /// The doctor's last name.
    /// </summary>
    public string LastName => model.LastName;

    /// <summary>
    /// An array of contact numbers for the doctor.
    /// </summary>
    public IReadOnlySet<string> ContactNumbers => model.ContactNumbers;

    /// <summary>
    /// An array of medical specialties that the doctor is trained in.
    /// </summary>
    public IReadOnlySet<string> Specialties => model.Specialties;

    public DoctorContract() : this(Doctor.Empty)
    {
    }

    public static implicit operator DoctorContract(Doctor model) => new(model);
}
