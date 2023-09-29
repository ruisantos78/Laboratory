namespace RuiSantos.Labs.GraphQL.Schemas;

[GraphQLName("Doctor")]
[GraphQLDescription("Represents a doctor.")]
public class DoctorSchema
{    
    [GraphQLDescription("Doctor Unique Identifier.")]
    public required string? Id { get; init; }

    [GraphQLDescription("Doctor license number.")]
    public required string License { get; init; }

    [GraphQLDescription("First name.")]
    public required string FirstName { get; init; }

    [GraphQLDescription("Last name.")]
    public required string LastName { get; init; }

    [GraphQLDescription("Email.")]
    public required string Email { get; init; }

    [GraphQLDescription("Contact numbers.")]
    public required List<string> Contacts { get; init; }

    [GraphQLDescription("Medical specialties.")]
    public required List<string> Specialties { get; init; }
}