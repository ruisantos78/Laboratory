using RuiSantos.Labs.GraphQL.Schemas;

namespace RuiSantos.Labs.GraphQL;

partial class Queries
{
    [GraphQLDescription("Get all medical specialties.")]
    public async Task<IEnumerable<MedicalSpecialtySchema>> GetSpecialties(
        [Service] IMedicalSpecialtySchemaAdapter adapter)
        => await adapter.FindAllAsync();
}