using RuiSantos.Labs.Core.Services;
using RuiSantos.Labs.Core.Models;
using RuiSantos.Labs.GraphQL.Schemas;

namespace RuiSantos.Labs.GraphQL;

public interface IDoctorSchemaAdapter {
    Task<DoctorSchema> CreateOrUpdateDoctorAsync(DoctorSchema doctor);
    Task<DoctorSchema> GetDoctorAsync(string license);
}

internal class DoctorSchemaAdapter: IDoctorSchemaAdapter
{
    private readonly IDoctorService management;

    public DoctorSchemaAdapter(IDoctorService management)
    {
        this.management = management;
    }

    private static DoctorSchema GetSchema(Doctor model) => new()
    {
        License = model.License,
        FirstName = model.FirstName,
        LastName = model.LastName,
        Email = model.Email,
        Contacts = model.ContactNumbers.ToList(),
        Specialties = model.Specialties.ToList()
    };

    private static Doctor GetModel(DoctorSchema schema) => new()
    {
        License = schema.License,
        FirstName = schema.FirstName,
        LastName = schema.LastName,
        Email = schema.Email,
        ContactNumbers = schema.Contacts.ToHashSet(),
        Specialties = schema.Specialties.ToHashSet()
    };

    public async Task<DoctorSchema> GetDoctorAsync(string license) {
        var doctor = await management.GetDoctorByLicenseAsync(license);     
        return GetSchema(doctor ?? new Doctor());
    }    

    public async Task<DoctorSchema> CreateOrUpdateDoctorAsync(DoctorSchema schema)
    {
        await management.CreateDoctorAsync(
            license: schema.License,
            firstName: schema.FirstName,
            lastName: schema.LastName,
            email: schema.Email,
            contactNumbers: schema.Contacts,
            specialties: schema.Specialties
        );
        return schema;
    }    
}
