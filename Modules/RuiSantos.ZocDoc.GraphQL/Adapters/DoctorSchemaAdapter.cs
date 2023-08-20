using RuiSantos.ZocDoc.Core.Services;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.GraphQL.Schemas;

namespace RuiSantos.ZocDoc.GraphQL;

public interface IDoctorSchemaAdapter {
    Task<DoctorSchema> GetDoctorAsync(string license);
}

internal class DoctorSchemaAdapter: IDoctorSchemaAdapter
{
    private readonly IDoctorService management;

    public DoctorSchemaAdapter(IDoctorService management)
    {
        this.management = management;
    }

    public async Task<DoctorSchema> GetDoctorAsync(string license) {
        var doctor = await management.GetDoctorByLicenseAsync(license);     
        return GetSchema(doctor ?? new Doctor());
    }    

    private static DoctorSchema GetSchema(Doctor doctor) => new()
    {
        License = doctor.License,
        FirstName = doctor.FirstName,
        LastName = doctor.LastName,
        Email = doctor.Email,
        Contacts = doctor.ContactNumbers.ToList(),
        Specialties = doctor.Specialties.ToList()
    };
}
