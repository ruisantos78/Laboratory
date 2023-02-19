namespace RuiSantos.ZocDoc.Core.Models;

public class Patient : Person
{
    public Guid Id { get; set; }
    public string SocialSecurityNumber { get; set; }
    public List<Appointment> Appointments { get; set; }

    public Patient()
    {
        Id = Guid.NewGuid();
        SocialSecurityNumber = string.Empty;
        Appointments= new List<Appointment>();
    }

    public Patient(string socialSecurityNumber, string email, string firstName, string lastName, IEnumerable<string> contactNumbers)
        : base(email, firstName, lastName, contactNumbers)
    {
        Id = Guid.NewGuid();
        SocialSecurityNumber = socialSecurityNumber;
        Appointments = new List<Appointment>();
    }
}

