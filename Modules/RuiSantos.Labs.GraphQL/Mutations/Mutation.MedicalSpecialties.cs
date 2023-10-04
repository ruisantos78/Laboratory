using RuiSantos.Labs.GraphQL.Adapters;
using RuiSantos.Labs.GraphQL.Schemas;

namespace RuiSantos.Labs.GraphQL.Mutations;

partial class Mutation
{
    [GraphQLDescription("Add medical specialties.")]
    public Task<IEnumerable<MedicalSpecialtySchema>> AddSpecialties(
      [GraphQLDescription("A list of medical specialties.")] List<string> descriptions,
      [Service] IMedicalSpecialtySchemaAdapter adapter)
      => adapter.StoreAsync(descriptions);

    [GraphQLDescription("Add medical specialties.")]
    public Task<IEnumerable<MedicalSpecialtySchema>> RemoveSpecialties(
      [GraphQLDescription("A medical specialty.")] string description,
      [Service] IMedicalSpecialtySchemaAdapter adapter)
      => adapter.RemoveAsync(description);
}