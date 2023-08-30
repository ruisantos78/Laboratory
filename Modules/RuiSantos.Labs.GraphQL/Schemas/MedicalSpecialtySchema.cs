namespace RuiSantos.Labs.GraphQL.Schemas;

[GraphQLName("Specialties")]
[GraphQLDescription("Medical Specialties")]
public class MedicalSpecialtySchema
{
    [GraphQLDescription("Description")]
    public required string Description { get; init; }
}
