namespace RuiSantos.Labs.Client.Models;

public class DoctorModel
{
    public string License { get; init; } = string.Empty;

    public string FirstName { get; init; } = string.Empty;

    public string LastName { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public List<string> Contacts { get; init; } = new();

    public List<string> Specialties { get; init; } = new();

    public string FullName => $"{LastName}, {FirstName}";

    public string EditUrl => $"/doctor/{License}";

    public DoctorModel()
    {
        
    }

    public DoctorModel(IGetDoctors_Doctors_Doctors value): this()
    {
        License = value.License;
        FirstName = value.FirstName;
        LastName = value.LastName;
        Email = value.Email;
        Contacts = value.Contacts.ToList();
        Specialties = value.Specialties.ToList();
    }
}
