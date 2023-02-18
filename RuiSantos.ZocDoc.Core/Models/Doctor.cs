namespace RuiSantos.ZocDoc.Core.Models;

public class Doctor: Person
{
    public Guid Id { get; set; }
    public string License { get; set; }
	public List<string> Specialties { get; set; }
    public List<OfficeHour> OfficeHours { get; set; }   
    public List<Appointment> Appointments { get; set; }

    public Doctor(): base()
	{
		Id = Guid.NewGuid();
		License = string.Empty;
        Specialties = new List<string>();
        OfficeHours = new List<OfficeHour>();
        Appointments = new List<Appointment>();
    }

    public Doctor(Guid id, string license, List<string> specialities,
        string email, string firstName, string lastName, List<string> contactNumbers)
        : base(email, firstName, lastName, contactNumbers)
    {
        Id = id;
        License = license;
        Specialties = specialities;
        OfficeHours = new List<OfficeHour>();
        Appointments = new List<Appointment>();
    }
}