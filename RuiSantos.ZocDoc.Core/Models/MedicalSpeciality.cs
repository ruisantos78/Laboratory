namespace RuiSantos.ZocDoc.Core.Models;

public class MedicalSpeciality
{
    public Guid Id { get; set; }
    public string Description { get; set; }

	public MedicalSpeciality()
	{
		this.Id = Guid.NewGuid();
		this.Description = String.Empty;
	}

	public MedicalSpeciality(string description)
	{
        this.Id = Guid.NewGuid();
        this.Description = description;
	}
}

