namespace RuiSantos.ZocDoc.Core.Models;

public class Doctor : Person
{
    public Guid Id { get; }
    public string License { get; set; }
    public List<string> Specialties { get; set; }
    public List<OfficeHour> OfficeHours { get; set; }
    public List<Appointment> Appointments { get; set; }

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