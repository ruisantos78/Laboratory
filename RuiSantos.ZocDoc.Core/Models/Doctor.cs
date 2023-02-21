namespace RuiSantos.ZocDoc.Core.Models;

public class Doctor : Person
{
    public static readonly Doctor Empty = new() { Id = Guid.Empty };

    public Guid Id { get; init; }
    public string License { get; init; }
    public List<string> Specialties { get; init; }
    public List<OfficeHour> OfficeHours { get; init; }
    public List<Appointment> Appointments { get; init; }

    public Doctor() : base()
    {
        Id = Guid.NewGuid();
        License = string.Empty;
        Specialties = new List<string>();
        OfficeHours = new List<OfficeHour>();
        Appointments = new List<Appointment>();
    }

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