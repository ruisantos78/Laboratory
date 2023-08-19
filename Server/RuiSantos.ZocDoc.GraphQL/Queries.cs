using RuiSantos.ZocDoc.Core.Managers;
using RuiSantos.ZocDoc.GraphQL.Schemas;

namespace RuiSantos.ZocDoc.GraphQL;

public class Queries
{
    [UseFiltering, UseSorting]
    [GraphQLDescription("Get information about a doctor.")]
    public async Task<DoctorSchema?> GetDoctor(
        [GraphQLDescription("License number")] string license,
        [Service] IDoctorManagement management)
    {
        var doctor = await management.GetDoctorByLicenseAsync(license);
        if (doctor is null)
            return null;

        return DoctorSchema.Create(doctor);
    }

    [UseFiltering, UseSorting]
    [GraphQLDescription("Get all medical specialties.")]
    public async Task<IQueryable<SpecialtySchema>> GetSpecialties([Service] IMedicalSpecialtiesManagement management)
    {
        var specialties = await management.GetMedicalSpecialitiesAsync();
        return specialties.Select(s => new SpecialtySchema { Specialty = s.Description }).AsQueryable();
    }
}