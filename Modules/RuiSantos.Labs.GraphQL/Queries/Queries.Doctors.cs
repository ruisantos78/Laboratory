using RuiSantos.Labs.GraphQL.Schemas;

namespace RuiSantos.Labs.GraphQL;

partial class Queries
{
    [GraphQLDescription("Get the amount of doctors.")]
    public async Task<long> CountDoctors(
        [Service] IDoctorSchemaAdapter adapter)
        => await adapter.CountAsync();

    [GraphQLDescription("Get information about all doctors.")]
    public async Task<IEnumerable<DoctorSchema>> GetDoctors(
        [GraphQLName("page")] Pagination page,
        [Service] IDoctorSchemaAdapter adapter)
        => await adapter.FindAllAsync(page.Take, page.From);

    [GraphQLDescription("Get information about a doctor.")]
    public async Task<DoctorSchema> GetDoctor(
        [GraphQLDescription("License number")] string license,
        [Service] IDoctorSchemaAdapter adapter) 
        => await adapter.FindAsync(license);
}