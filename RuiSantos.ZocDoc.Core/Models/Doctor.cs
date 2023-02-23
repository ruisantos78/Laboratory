namespace RuiSantos.ZocDoc.Core.Models;

/// <sumary>
/// A doctor is a person who is able to perform medical procedures.
/// </sumary>
public class Doctor : Person
{
    /// <summary>
    /// An empty doctor.
    /// </summary>
    public static readonly Doctor Empty = new() { Id = Guid.Empty };

    /// <summary>
    /// The unique identifier of the doctor.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// The license number of the doctor.
    /// </summary>
    public string License { get; init; }

    /// <summary>
    /// The specialties of the doctor.
    /// </summary>
    public List<string> Specialties { get; init; }

    /// <summary>
    /// The office hours of the doctor.
    /// </summary>
    public List<OfficeHour> OfficeHours { get; init; }

    /// <summary>
    /// The appointments of the doctor.
    /// </summary>
    public List<Appointment> Appointments { get; init; }


    /// <summary>
    /// Creates a new instance of the <see cref="Doctor"/> class.
    /// </summary>
    public Doctor() : base()
    {
        Id = Guid.NewGuid();
        License = string.Empty;
        Specialties = new List<string>();
        OfficeHours = new List<OfficeHour>();
        Appointments = new List<Appointment>();
    }

    /// <summary>
    /// Creates a new instance of the <see cref="Doctor"/> class.
    /// </summary>
    /// <param name="license">The license number of the doctor.</param>
    /// <param name="email">The email of the doctor.</param>
    /// <param name="firstName">The first name of the doctor.</param>
    /// <param name="lastName">The last name of the doctor.</param>
    /// <param name="contactNumbers">The contact numbers of the doctor.</param>
    /// <param name="specialities">The specialities of the doctor.</param>
    public Doctor(string license, string email,
        string firstName, string lastName, IEnumerable<string> contactNumbers, IEnumerable<string> specialities)
        : base(email, firstName, lastName, contactNumbers)
    {
        Id = Guid.NewGuid();
        License = license;
        Specialties = specialities.ToList();
        OfficeHours = new List<OfficeHour>();
        Appointments = new List<Appointment>();
    }
}