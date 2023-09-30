using RuiSantos.Labs.GraphQL.Adapters;
using RuiSantos.Labs.GraphQL.Schemas;

namespace RuiSantos.Labs.GraphQL.Mutations;

partial class Mutation
{
    [GraphQLDescription("Create/update a doctor.")]
    public Task<DoctorSchema> SetDoctor(
      [GraphQLDescription("The doctor.")] DoctorSchema doctor,
      [Service] IDoctorSchemaAdapter adapter)
      => adapter.StoreAsync(doctor);
}

