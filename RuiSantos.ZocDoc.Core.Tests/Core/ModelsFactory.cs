using RuiSantos.ZocDoc.Core.Models;

namespace RuiSantos.ZocDoc.Core.Tests.Core;

internal static class ModelsFactory
{
    public static Patient CreatePatient() => new("123-45-6789", "john@patient.com", "Jhon", "Doe", Array.Empty<string>());
    public static Patient CreatePatient(string socialNumber) => new(socialNumber, "john@patient.com", "Jhon", "Doe", Array.Empty<string>());

    public static Doctor CreateDoctor() => new("ABC123", "mary@doctor.com", "Mary", "Jane", Array.Empty<string>(), new[] { "Cardiology" });
    public static Doctor CreateDoctor(string medicalLincense) => new(medicalLincense, "mary@doctor.com", "Mary", "Jane", Array.Empty<string>(), new[] { "Cardiology" });
}
