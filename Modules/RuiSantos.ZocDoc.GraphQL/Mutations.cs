using RuiSantos.ZocDoc.GraphQL.Schemas;

namespace RuiSantos.ZocDoc.GraphQL;

public class Mutations
{
    [GraphQLDescription("Add medical specialties.")]
    public async Task<IEnumerable<MedicalSpecialtySchema>> AddSpecialties(
      [GraphQLDescription("A list of medical specialties.")] List<string> descriptions,
      [Service] IMedicalSpecialtySchemaAdapter adapter)
      => await adapter.CreateMedicalSpecialtiesAsync(descriptions);

    [GraphQLDescription("Create/update a doctor.")]
    public async Task<DoctorSchema> SetDoctor(
      [GraphQLDescription("The doctor.")] DoctorSchema doctor,
      [Service] IDoctorSchemaAdapter adapter)
      => await adapter.CreateOrUpdateDoctorAsync(doctor);
}