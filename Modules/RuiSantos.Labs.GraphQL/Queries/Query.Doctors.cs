using RuiSantos.Labs.GraphQL.Adapters;
using RuiSantos.Labs.GraphQL.Schemas;

namespace RuiSantos.Labs.GraphQL.Queries;

partial class Query
{
    [GraphQLDescription("Get information about all doctors.")]
    public Task<IEnumerable<DoctorSchema>> GetDoctors(
        [GraphQLName("page")] Pagination page,
        [Service] IDoctorSchemaAdapter adapter)
        => adapter.FindAllAsync(page.Take, page.From);

    [GraphQLDescription("Get information about a doctor.")]
    public Task<DoctorSchema> GetDoctor(
        [GraphQLDescription("id")] string id,
        [Service] IDoctorSchemaAdapter adapter) 
        => adapter.FindAsync(id);
}