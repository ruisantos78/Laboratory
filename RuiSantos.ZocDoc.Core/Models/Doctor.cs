using System;
namespace RuiSantos.ZocDoc.Core.Models;

public class Doctor: Person
{
    public Guid Id { get; set; }
    public string License { get; set; }
	public List<String> Specialties { get; set; }

	public Doctor(): base()
	{
		this.Id = Guid.NewGuid();
		this.License = string.Empty;
        this.Specialties = new List<String>();
	}

    public Doctor(Guid id, string license, List<string> specialities,
        string email, string firstName, string lastName, List<string> contactNumbers)
        : base(email, firstName, lastName, contactNumbers)
    {
        Id = id;
        License = license;
        Specialties = specialities;
    }
}