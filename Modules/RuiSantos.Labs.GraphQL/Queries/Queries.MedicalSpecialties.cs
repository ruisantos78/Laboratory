using RuiSantos.Labs.GraphQL.Schemas;

namespace RuiSantos.Labs.GraphQL;

partial class Queries
{
    [UseFiltering, UseSorting]
    [GraphQLDescription("Get all medical specialties.")]
    public async Task<IQueryable<MedicalSpecialtySchema>> GetSpecialties(
        [Service] IMedicalSpecialtySchemaAdapter adapter)
        => await adapter.FindAllAsync();
}