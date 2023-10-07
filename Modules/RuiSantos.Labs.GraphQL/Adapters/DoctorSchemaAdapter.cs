using RuiSantos.Labs.Core.Models;
using RuiSantos.Labs.Core.Services;
using RuiSantos.Labs.GraphQL.Schemas;

namespace RuiSantos.Labs.GraphQL.Adapters;

[Adapter(typeof(DoctorSchemaAdapter))]
public interface IDoctorSchemaAdapter {
    Task<DoctorSchema> StoreAsync(DoctorSchema doctor);
    Task<DoctorSchema?> FindAsync(string license);
    Task<DoctorsCollectionSchema> FindAllAsync(int take, string? paginationToken = null);
}

internal class DoctorSchemaAdapter: AdapterModelSchema<Doctor, DoctorSchema>, IDoctorSchemaAdapter
{
    private readonly IDoctorService _service;

    public DoctorSchemaAdapter(IDoctorService service)
    {
        _service = service;
    }

    protected override DoctorSchema GetSchema(Doctor model) => new()
    {
        License = model.License,
        FirstName = model.FirstName,
        LastName = model.LastName,
        Email = model.Email,
        Contacts = model.ContactNumbers.ToList(),
        Specialties = model.Specialties.ToList()
    };

    protected override Doctor GetModel(DoctorSchema schema) => new()
    {
        License = schema.License,
        FirstName = schema.FirstName,
        LastName = schema.LastName,
        Email = schema.Email,
        ContactNumbers = schema.Contacts.ToHashSet(),
        Specialties = schema.Specialties.ToHashSet()
    };

    public async Task<DoctorSchema?> FindAsync(string license) {
        var doctor = await _service.GetDoctorByLicenseAsync(license);
        if (doctor is null)
            return default;

        return GetSchema(doctor);
    }

    public async Task<DoctorsCollectionSchema> FindAllAsync(int take, string? paginationToken)
    {
        var pagination = await _service.GetAllDoctorsAsync(take, paginationToken);
        return new DoctorsCollectionSchema
        {
            Doctors = pagination.Models.Select(GetSchema).ToList(),
            PaginationToken = pagination.PaginationToken
        };
    }

    public async Task<DoctorSchema> StoreAsync(DoctorSchema schema)
    {
        await _service.CreateDoctorAsync(
            license: schema.License,
            firstName: schema.FirstName,
            lastName: schema.LastName,
            email: schema.Email,
            contactNumbers: schema.Contacts,
            specialties: schema.Specialties
        );

        var entity = await _service.GetDoctorByLicenseAsync(schema.License);
        if (entity is null)
            throw new InvalidOperationException("Failure to store the doctor");

        return GetSchema(entity);
    }
}
