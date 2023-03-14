namespace RuiSantos.ZocDoc.Core.Models;

/// <summary>
/// Represents a patient.
/// </summary>
public class Patient : Person
{
    /// <summary>
    /// An empty patient.
    /// </summary>
    public static readonly Patient Empty = new() { Id = Guid.Empty };

    /// <summary>
    /// The patient identifier.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// The patient's social security number.
    /// </summary>
    public string SocialSecurityNumber { get; init; }

    /// <summary>
    /// The patient's appointments. (Not persisted)
    /// </summary>
    public List<Appointment> Appointments { get; init; }


    /// <summary>
    /// Creates an empty patient. (Used for serialization)
    /// </summary>
    public Patient()
    {
        Id = Guid.NewGuid();
        SocialSecurityNumber = string.Empty;
        Appointments = new List<Appointment>();
    }

    /// <summary>
    /// Creates a new patient.
    /// </summary>
    /// <param name="socialSecurityNumber">The patient's social security number.</param>
    /// <param name="email">The patient's email address.</param>
    /// <param name="firstName">The patient's first name.</param>
    /// <param name="lastName">The patient's last name.</param>
    /// <param name="contactNumbers">The patient's contact numbers.</param>
    public Patient(string socialSecurityNumber, string email, string firstName, string lastName, IEnumerable<string> contactNumbers)
        : base(email, firstName, lastName, contactNumbers)
    {
        Id = Guid.NewGuid();
        SocialSecurityNumber = socialSecurityNumber;
        Appointments = new List<Appointment>();
    }
}

