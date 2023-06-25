namespace RuiSantos.ZocDoc.Core.Tests.Factories;

internal class SpecialtiesBuilder
{
    public static MedicalSpecialty Single(string description) => new(description);
    
    public static SpecialtiesBuilder With(params string[] specialties) => new(specialties);

    public static SpecialtiesBuilder Dummy() => With(
        "Cardiology",
        "Dermatology",
        "Endocrinology",
        "Gastroenterology",
        "Geriatrics");

    private readonly List<MedicalSpecialty> specialties;

    private SpecialtiesBuilder(string[] specialties)
    {
        this.specialties = specialties?.Select(s => new MedicalSpecialty(s)).ToList()
            ?? new List<MedicalSpecialty>();
    }

    public List<MedicalSpecialty> Build() => specialties;

    public SpecialtiesBuilder AddSpecialties(params string[] descriptions)
    {
        specialties.AddRange(descriptions.Select(d => new MedicalSpecialty(d)));
        return this;
    }
}