using RuiSantos.Labs.Core.Models;

namespace RuiSantos.Labs.Api.Contracts;

/// <summary>
/// Contract for a doctor's information
/// </summary>
public class DoctorContract
{
    /// <summary>
    /// The doctor's license number.
    /// </summary>
    public string License { get; init; }

    /// <summary>
    /// The doctor's email.
    /// </summary>
    public string Email { get; init; }

    /// <summary>
    /// The doctor's first name.
    /// </summary>
    public string FirstName { get; init; }

    /// <summary>
    /// The doctor's last name.
    /// </summary>
    public string LastName { get; init; }

    /// <summary>
    /// An array of contact numbers for the doctor.
    /// </summary>
    public IReadOnlyList<string> ContactNumbers { get; init; }

    /// <summary>
    /// An array of medical specialties that the doctor is trained in.
    /// </summary>
    public IReadOnlyList<string> Specialties { get; init; }

    public DoctorContract() : this(Doctor.Empty)
    {
    }

    public DoctorContract(Doctor model)
    {
        License = model.License;
        Email = model.Email;
        FirstName = model.FirstName;
        LastName = model.LastName;
        ContactNumbers = model.ContactNumbers.ToArray();
        Specialties = model.Specialties.ToArray();
    }
}
