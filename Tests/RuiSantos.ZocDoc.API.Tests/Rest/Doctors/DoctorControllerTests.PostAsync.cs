using System.Net;
using FluentAssertions;
using RuiSantos.ZocDoc.Api.Contracts;
using RuiSantos.ZocDoc.Data.Dynamodb.Entities;

using static RuiSantos.ZocDoc.Data.Dynamodb.Mappings.ClassMapConstants;

namespace RuiSantos.ZocDoc.API.Tests.Rest;

partial class DoctorControllerTests
{
    [Fact(DisplayName = "Should return a success message if the doctor is successfully created.")]
    public async Task ShouldReturnSuccessMessageWhenDoctorIsCreated()
    {
        // Arrange
        var request = new DoctorContract
        {
            License = "ZZZ123",
            FirstName = "Joe",
            LastName = "Doe",
            Email = "joe.doe@email.net",
            ContactNumbers = new[] { "1-555-5555" },
            Specialties = new[] { "Cardiology" }
        };

        // Act
        var response = await client.PostAsync("/Doctor/", request, output);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var doctor = await context.FindAsync<DoctorDto>(DoctorLicenseIndexName, "ZZZ123");
        doctor.FirstName.Should().Be("Joe");
        doctor.LastName.Should().Be("Doe");
        doctor.Email.Should().Be("joe.doe@email.net");
        doctor.ContactNumbers.Should().ContainSingle("1-555-5555");

        var specialties = await context.QueryAsync<DoctorSpecialtyDto>(doctor.Id)
            .GetRemainingAsync();

        specialties.Should().HaveCount(1).And.ContainSingle(ds => ds.Specialty == "Cardiology");

        // Teardown
        await context.DeleteAsync(doctor);
    }
}