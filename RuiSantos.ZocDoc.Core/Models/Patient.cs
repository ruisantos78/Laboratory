namespace RuiSantos.ZocDoc.Core.Models;

public class Patient : Person
{
    public static readonly Patient Empty = new() { Id = Guid.Empty };

    public Guid Id { get; init; }
    public string SocialSecurityNumber { get; init; }
    public List<Appointment> Appointments { get; init; }

    public Patient()
    {
        Id = Guid.NewGuid();
        SocialSecurityNumber = string.Empty;
        Appointments = new List<Appointment>();
    }    

    public Patient(string socialSecurityNumber, string email, string firstName, string lastName, IEnumerable<string> contactNumbers)
        : base(email, firstName, lastName, contactNumbers)
    {
        Id = Guid.NewGuid();
        SocialSecurityNumber = socialSecurityNumber;
        Appointments = new List<Appointment>();
    }
}

