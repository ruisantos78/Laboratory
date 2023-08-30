namespace RuiSantos.Labs.Core.Models;

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
    /// Creates an empty patient.
    /// </summary>
    public Patient()
    {
        SocialSecurityNumber = string.Empty;
    }
}

