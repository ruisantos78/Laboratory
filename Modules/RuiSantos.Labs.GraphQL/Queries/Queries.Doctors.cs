using RuiSantos.Labs.GraphQL.Schemas;

namespace RuiSantos.Labs.GraphQL;

partial class Queries
{
    [UseFiltering, UseSorting]
    [GraphQLDescription("Get information about a doctor.")]
    public async Task<DoctorSchema> GetDoctor(
        [GraphQLDescription("License number")] string license,
        [Service] IDoctorSchemaAdapter adapter) 
        => await adapter.FindAsync(license);
}
