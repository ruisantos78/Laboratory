﻿namespace RuiSantos.Labs.GraphQL.Schemas;

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
}

[GraphQLName("DoctorsCollection")]
[GraphQLDescription("Represents a list of doctors.")]
public class DoctorsCollectionSchema
{
    [GraphQLDescription("List of doctors.")]
    public required List<DoctorSchema> Doctors { get; init; }

    [GraphQLDescription("Pagination token.")]
    public string? PaginationToken { get; init; }
}