using RuiSantos.ZocDoc.GraphQL.Schemas;

namespace RuiSantos.ZocDoc.GraphQL;

public class Mutations
{
    [GraphQLDescription("Create a medical specialty.")]
    public async Task<MedicalSpecialtySchema> CreateSpecialty(
      [GraphQLDescription("The description of  the specialty.")] string description,
      [Service] IMedicalSpecialtySchemaAdapter adapter)
      => await adapter.CreateMedicalSpecialtyAsync(description);
    
    [GraphQLDescription("Create a list of medical specialties.")]
    public async Task<IEnumerable<MedicalSpecialtySchema>> CreateSpecialties(
      [GraphQLDescription("A list of medical specialties.")] List<string> descriptions,
      [Service] IMedicalSpecialtySchemaAdapter adapter)
      => await adapter.CreateMedicalSpecialtiesAsync(descriptions);
}