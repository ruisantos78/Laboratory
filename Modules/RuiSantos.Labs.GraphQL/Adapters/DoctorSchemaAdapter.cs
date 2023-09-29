﻿using RuiSantos.Labs.Core.Services;
using RuiSantos.Labs.Core.Models;
using RuiSantos.Labs.GraphQL.Schemas;
using RuiSantos.Labs.GraphQL.Services;

namespace RuiSantos.Labs.GraphQL;

[Adapter(typeof(DoctorSchemaAdapter))]
public interface IDoctorSchemaAdapter {
    Task<DoctorSchema> StoreAsync(DoctorSchema doctor);
    Task<DoctorSchema> FindAsync(string id);
    Task<IEnumerable<DoctorSchema>> FindAllAsync(int take, string? from = null);
}

internal class DoctorSchemaAdapter: IDoctorSchemaAdapter
{
    private readonly ISecurity security;
    private readonly IDoctorService service;

    public DoctorSchemaAdapter(
        ISecurity security,
        IDoctorService service)
    {
        this.security = security;
        this.service = service;
    }

    private DoctorSchema GetSchema(Doctor model) => new()
    {
        Id = security.Encode(model.Id),
        License = model.License,
        FirstName = model.FirstName,
        LastName = model.LastName,
        Email = model.Email,
        Contacts = model.ContactNumbers.ToList(),
        Specialties = model.Specialties.ToList()
    };

    private Doctor GetModel(DoctorSchema schema) => new()
    {
        Id = security.Decode(schema.Id),
        License = schema.License,
        FirstName = schema.FirstName,
        LastName = schema.LastName,
        Email = schema.Email,
        ContactNumbers = schema.Contacts.ToHashSet(),
        Specialties = schema.Specialties.ToHashSet()
    };

    public async Task<DoctorSchema> FindAsync(string id) {
        var doctorId = security.Decode(id);
        var doctor = await service.GetDoctorAsync(doctorId);     
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
}
