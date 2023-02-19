using RuiSantos.ZocDoc.Core.Models;

namespace RuiSantos.ZocDoc.Core.Tests.Factories;

internal static class SpecialtyFactory
{
    public static List<MedicalSpeciality> Create() => new()
    {
        new("Cardiology"),
        new("Dermatology"),
        new("Endocrinology"),
        new("Gastroenterology"),
        new("Geriatrics")
    };

    public static List<MedicalSpeciality> Create(params string[] descriptions) => descriptions.Select(s => new MedicalSpeciality(s)).ToList();
}
