using System;
namespace RuiSantos.ZocDoc.Core.Models;

public class Doctor: Person
{
    public string? Id { get; set; }
    public string License { get; set; }
	public HashSet<MedicalSpeciality> Specialities { get; set; }

	public Doctor()
	{
		this.License = string.Empty;
        this.Specialities = new HashSet<MedicalSpeciality>();
	}
}