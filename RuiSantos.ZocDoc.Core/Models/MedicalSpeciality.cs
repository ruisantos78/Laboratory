namespace RuiSantos.ZocDoc.Core.Models;

public class MedicalSpeciality
{
    public Guid Id { get; init; }
    public string Description { get; set; }

    public MedicalSpeciality()
    {
        Id = Guid.NewGuid();
        Description = String.Empty;
    }

    public MedicalSpeciality(string description)
    {
        Id = Guid.NewGuid();
        Description = description;
    }
}

