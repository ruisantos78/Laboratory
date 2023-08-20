namespace RuiSantos.ZocDoc.GraphQL.Schemas;

[GraphQLName("Medical Specialty")]
[GraphQLDescription("Represents a medical specialty.")]
public class MedicalSpecialtySchema
{
    [GraphQLDescription("Medical specialty description")]
    public required string Description { get; init; }
}