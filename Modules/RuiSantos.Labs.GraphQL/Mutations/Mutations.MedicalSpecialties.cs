using RuiSantos.Labs.GraphQL.Schemas;

namespace RuiSantos.Labs.GraphQL;

partial class Mutations
{
    [GraphQLDescription("Add medical specialties.")]
    public async Task<IEnumerable<MedicalSpecialtySchema>> AddSpecialties(
      [GraphQLDescription("A list of medical specialties.")] List<string> descriptions,
      [Service] IMedicalSpecialtySchemaAdapter adapter)
      => await adapter.StoreAsync(descriptions);

    [GraphQLDescription("Add medical specialties.")]
    public async Task<IEnumerable<MedicalSpecialtySchema>> RemoveSpecialties(
      [GraphQLDescription("A medical specialty.")] string description,
      [Service] IMedicalSpecialtySchemaAdapter adapter)
      => await adapter.RemoveAsync(description);
}