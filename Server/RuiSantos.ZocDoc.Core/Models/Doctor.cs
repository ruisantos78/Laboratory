namespace RuiSantos.ZocDoc.Core.Models;

/// <sumary>
/// A doctor is a person who is able to perform medical procedures.
/// </sumary>
public class Doctor : Person
{
    /// <summary>
    /// An empty doctor.
    /// </summary>
    public static readonly Doctor Empty = new();

    /// <summary>
    /// The license number of the doctor.
    /// </summary>
    public string License { get; set; }

    /// <summary>
    /// The specialties of the doctor.
    /// </summary>
    public HashSet<string> Specialties { get; init; }

    /// <summary>
    /// The office hours of the doctor.
    /// </summary>
    public HashSet<OfficeHour> OfficeHours { get; init; }

    /// <summary>
    /// The appointments of the doctor.
    /// </summary>
    public HashSet<Appointment> Appointments { get; init; }


    /// <summary>
    /// Creates a new instance of the <see cref="Doctor"/> class.
    /// </summary>
    public Doctor() : base()
    {
        License = string.Empty;
        Specialties = new HashSet<string>();
        OfficeHours = new HashSet<OfficeHour>();
        Appointments = new HashSet<Appointment>();
    }
}