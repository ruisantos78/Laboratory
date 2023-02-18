namespace RuiSantos.ZocDoc.Core.Models;

public class MedicalSpeciality
{
    public string? Id { get; set; }
    public string Description { get; set; }

	public MedicalSpeciality()
	{
		this.Description = String.Empty;
	}
}

