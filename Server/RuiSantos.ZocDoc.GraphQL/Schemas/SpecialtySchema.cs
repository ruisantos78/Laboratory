namespace RuiSantos.ZocDoc.GraphQL.Schemas;

[GraphQLName("Medical Specialty")]
[GraphQLDescription("Represents a medical specialty.")]
public class SpecialtySchema
{
    public required string Specialty { get; init; }
}

