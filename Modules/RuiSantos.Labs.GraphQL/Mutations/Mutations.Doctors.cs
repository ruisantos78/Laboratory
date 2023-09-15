using RuiSantos.Labs.GraphQL.Schemas;

namespace RuiSantos.Labs.GraphQL;

partial class Mutations
{
    [GraphQLDescription("Create/update a doctor.")]
    public async Task<DoctorSchema> SetDoctor(
      [GraphQLDescription("The doctor.")] DoctorSchema doctor,
      [Service] IDoctorSchemaAdapter adapter)
      => await adapter.StoreAsync(doctor);
}

