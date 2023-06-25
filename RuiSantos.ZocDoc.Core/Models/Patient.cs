namespace RuiSantos.ZocDoc.Core.Models;

/// <summary>
/// Represents a patient.
/// </summary>
public class Patient : Person
{
    /// <summary>
    /// An empty patient.
    /// </summary>
    public static readonly Patient Empty = new();

    /// <summary>
    /// The patient's social security number.
    /// </summary>
    public string SocialSecurityNumber { get; set; }

    /// <summary>
    /// The patient's appointments. (Not persisted)
    /// </summary>
    public HashSet<Appointment> Appointments { get; init; }


    /// <summary>
    /// Creates an empty patient.
    /// </summary>
    public Patient()
    {
        SocialSecurityNumber = string.Empty;
        Appointments = new HashSet<Appointment>();
    }
}

