using RuiSantos.ZocDoc.GraphQL.Schemas;

namespace RuiSantos.ZocDoc.GraphQL;

public class Mutations
{
    [GraphQLDescription("Add medical specialties.")]
    public async Task<IEnumerable<MedicalSpecialtySchema>> AddSpecialties(
      [GraphQLDescription("A list of medical specialties.")] List<string> descriptions,
      [Service] IMedicalSpecialtySchemaAdapter adapter)
      => await adapter.CreateMedicalSpecialtiesAsync(descriptions);
}