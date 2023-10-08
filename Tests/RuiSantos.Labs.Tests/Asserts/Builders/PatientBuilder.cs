using RuiSantos.Labs.Core.Models;

namespace RuiSantos.Labs.Tests.Asserts.Builders;

internal class PatientBuilder
{
    private readonly Patient _patient;

    public PatientBuilder(Guid? id = null)
    {
        _patient = new()
        {
            Id = id ?? Guid.NewGuid()
        };
    }
    
    public Patient Build() => _patient;
    
    public PatientBuilder With(string? securityNumber = null, string? email = null, string? firstName = null, string? lastName = null)
    {
        _patient.SocialSecurityNumber = securityNumber ?? NewSocialSecurityNumber();
        _patient.Email = email ?? "patient@email.com";
        _patient.FirstName = firstName ?? "Fake";
        _patient.LastName = lastName ?? "Patient";
        
        return this;
    }
    
    private static string NewSocialSecurityNumber()
    {
        var random = new Random();
        return $"{random.Next(999):000}-{random.Next(99):00}-{random.Next(9999):0000}";
    }
}