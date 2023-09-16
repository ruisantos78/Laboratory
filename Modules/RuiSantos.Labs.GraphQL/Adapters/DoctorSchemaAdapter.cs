using RuiSantos.Labs.Core.Services;
using RuiSantos.Labs.Core.Models;
using RuiSantos.Labs.GraphQL.Schemas;

namespace RuiSantos.Labs.GraphQL;

[Adapter(typeof(DoctorSchemaAdapter))]
public interface IDoctorSchemaAdapter {
    Task<DoctorSchema> StoreAsync(DoctorSchema doctor);
    Task<DoctorSchema> FindAsync(string license);
    Task<IEnumerable<DoctorSchema>> FindAllAsync(int take, string? from = null);
    Task<long> CountAsync();
}

internal class DoctorSchemaAdapter: IDoctorSchemaAdapter
{
    private readonly IDoctorService service;

    public DoctorSchemaAdapter(IDoctorService service)
    {
        this.service = service;
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

    public async Task<DoctorSchema> FindAsync(string license) {
        var doctor = await service.GetDoctorByLicenseAsync(license);     
        return GetSchema(doctor ?? new Doctor());
    }

    public async Task<IEnumerable<DoctorSchema>> FindAllAsync(int take, string? from = null)
    {
        var doctors = await service.GetAllDoctors(take, from);
        return doctors.Select(GetSchema);
    }

    public async Task<DoctorSchema> StoreAsync(DoctorSchema schema)
    {
        await service.CreateDoctorAsync(
            license: schema.License,
            firstName: schema.FirstName,
            lastName: schema.LastName,
            email: schema.Email,
            contactNumbers: schema.Contacts,
            specialties: schema.Specialties
        );
        return schema;
    }

    public async Task<long> CountAsync()
    {
        return await service.CountDoctorsAsync();
    }
}
