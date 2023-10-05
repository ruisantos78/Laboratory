using RuiSantos.Labs.Core.Models;
using RuiSantos.Labs.Core.Services;
using RuiSantos.Labs.GraphQL.Schemas;
using RuiSantos.Labs.GraphQL.Services;

namespace RuiSantos.Labs.GraphQL.Adapters;

[Adapter(typeof(DoctorSchemaAdapter))]
public interface IDoctorSchemaAdapter {
    Task<DoctorSchema> StoreAsync(DoctorSchema doctor);
    Task<DoctorSchema> FindAsync(string id);
    Task<DoctorsCollectionSchema> FindAllAsync(int take, string? paginationToken = null);
}

internal class DoctorSchemaAdapter: AdapterModelSchema<Doctor, DoctorSchema>, IDoctorSchemaAdapter
{
    private readonly ISecurity _security;
    private readonly IDoctorService _service;

    public DoctorSchemaAdapter(
        ISecurity security,
        IDoctorService service)
    {
        _security = security;
        _service = service;
    }

    protected override DoctorSchema GetSchema(Doctor model) => new()
    {
        Id = _security.Encode(model.Id),
        License = model.License,
        FirstName = model.FirstName,
        LastName = model.LastName,
        Email = model.Email,
        Contacts = model.ContactNumbers.ToList(),
        Specialties = model.Specialties.ToList()
    };

    protected override Doctor GetModel(DoctorSchema schema) => new()
    {
        Id = _security.Decode(schema.Id),
        License = schema.License,
        FirstName = schema.FirstName,
        LastName = schema.LastName,
        Email = schema.Email,
        ContactNumbers = schema.Contacts.ToHashSet(),
        Specialties = schema.Specialties.ToHashSet()
    };

    public async Task<DoctorSchema> FindAsync(string id) {
        var doctorId = _security.Decode(id);
        var doctor = await _service.GetDoctorAsync(doctorId);
        return GetSchema(doctor ?? new Doctor());
    }

    public async Task<DoctorsCollectionSchema> FindAllAsync(int take, string? paginationToken)
    {
        var pagination = await _service.GetAllDoctors(take, paginationToken);
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
        return schema;
    }
}
