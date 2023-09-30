using RuiSantos.Labs.GraphQL.Adapters;
using RuiSantos.Labs.GraphQL.Schemas;

namespace RuiSantos.Labs.GraphQL.Queries;

partial class Query
{
    [GraphQLDescription("Get all medical specialties.")]
    public Task<IEnumerable<MedicalSpecialtySchema>> GetSpecialties(
        [Service] IMedicalSpecialtySchemaAdapter adapter)
        => adapter.FindAllAsync();
}