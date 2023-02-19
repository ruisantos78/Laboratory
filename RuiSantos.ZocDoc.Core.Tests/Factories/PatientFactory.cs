using RuiSantos.ZocDoc.Core.Models;

namespace RuiSantos.ZocDoc.Core.Tests.Factories;

internal static class PatientFactory
{
    public static Patient Create(
        string socialNumber = "123-45-6789",
        string email = "john@patient.com",
        string firstName = "Jhon",
        string lastName = "Doe",
        string[]? contactNumbers = null
    )
    => new(socialNumber, email, firstName, lastName, contactNumbers ?? Array.Empty<string>());
}
