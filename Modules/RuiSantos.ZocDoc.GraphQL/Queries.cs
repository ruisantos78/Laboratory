using RuiSantos.ZocDoc.GraphQL.Schemas;

namespace RuiSantos.ZocDoc.GraphQL;

public class Queries
{
    [UseSorting]
    [GraphQLDescription("Get all medical specialties.")]
    public async Task<IQueryable<MedicalSpecialtySchema>> GetSpecialties(
        [Service] IMedicalSpecialtySchemaAdapter adapter)
        => await adapter.GetMedicalSpecialtiesAsync();

    [UseFiltering, UseSorting]
    [GraphQLDescription("Get information about a doctor.")]
    public async Task<DoctorSchema> GetDoctor(
        [GraphQLDescription("License number")] string license,
        [Service] IDoctorSchemaAdapter adapter) 
        => await adapter.GetDoctorAsync(license);
}