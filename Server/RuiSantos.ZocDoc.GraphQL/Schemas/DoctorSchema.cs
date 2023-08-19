using RuiSantos.ZocDoc.Core.Models;

namespace RuiSantos.ZocDoc.GraphQL.Schemas;

[GraphQLName("Doctor")]
[GraphQLDescription("Represents a doctor.")]
public class DoctorSchema
{
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
    
    public static DoctorSchema Create(Doctor doctor) => new()
    {
        License = doctor.License,
        FirstName = doctor.FirstName,
        LastName = doctor.LastName,
        Email = doctor.Email,
        Contacts = doctor.ContactNumbers.ToList(),
        Specialties = doctor.Specialties.ToList()
    };
}

