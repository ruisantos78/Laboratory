using RuiSantos.Labs.Core.Models;
using RuiSantos.Labs.Core.Services;
using RuiSantos.Labs.GraphQL.Schemas;

namespace RuiSantos.Labs.GraphQL.Adapters;

[Adapter(typeof(DoctorSchemaAdapter))]
public interface IDoctorSchemaAdapter
{
    Task<DoctorSchema> StoreAsync(DoctorSchema doctor);
    Task<DoctorSchema?> FindAsync(string license);
    Task<DoctorsCollectionSchema> FindAllAsync(int take, string? paginationToken = null);
}

internal class DoctorSchemaAdapter : IDoctorSchemaAdapter
{
    private readonly IDoctorService _service;

    public DoctorSchemaAdapter(IDoctorService service)
    {
        _service = service;
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

    public async Task<DoctorSchema?> FindAsync(string license)
    {
        return await _service.GetDoctorByLicenseAsync(license) is { } doctor
            ? GetSchema(doctor)
            : default;
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

        return await _service.GetDoctorByLicenseAsync(schema.License) is { } entity
            ? GetSchema(entity)
            : throw new InvalidOperationException("Failure to store the doctor");
    }
}
