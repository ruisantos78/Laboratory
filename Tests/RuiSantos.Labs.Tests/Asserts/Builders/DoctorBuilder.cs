using RuiSantos.Labs.Core.Models;

namespace RuiSantos.Labs.Tests.Asserts.Builders;

internal class DoctorBuilder
{
    private readonly Doctor model = new();
    
    public DoctorBuilder With(string? license = null, string? email = null, string? firstName = null, string? lastName = null)
    {
        model.License = license ?? model.License;
        model.Email = email ?? model.Email;
        model.FirstName = firstName ?? model.FirstName;
        model.LastName = lastName ?? model.LastName;
        
        return this;
    }
    
    public DoctorBuilder AddSpecialties(IEnumerable<string> specialties)
    {
        specialties.ToList().ForEach(x => model.Specialties.Add(x));
        return this;
    }
    
    public DoctorBuilder AddContacNumbers(IEnumerable<string> contactNumbers)
    {
        contactNumbers.ToList().ForEach(x => model.ContactNumbers.Add(x));
        return this;
    }
    
    public Doctor Build()
    {
        return model;
    }
}