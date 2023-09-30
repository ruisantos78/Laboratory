using System.Net;
using FluentAssertions;
using RuiSantos.Labs.Api.Contracts;
using RuiSantos.Labs.Api.Tests.Extensions;
using RuiSantos.Labs.Data.Dynamodb.Entities;

using static RuiSantos.Labs.Data.Dynamodb.Mappings.MappingConstants;

namespace RuiSantos.Labs.Api.Tests.Rest.Doctors;

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
        var response = await Client.PostAsync("/Doctor/", request, Output);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var doctor = await Context.FindAsync<DoctorEntity>("ZZZ123", DoctorLicenseIndexName);
        doctor.FirstName.Should().Be("Joe");
        doctor.LastName.Should().Be("Doe");
        doctor.Email.Should().Be("joe.doe@email.net");
        doctor.ContactNumbers.Should().ContainSingle("1-555-5555");

        var specialties = await Context.FindAllAsync<DoctorSpecialtyEntity>(doctor.Id);

        specialties.Should().HaveCount(1).And.ContainSingle(ds => ds.Specialty == "Cardiology");

        // Teardown
        await Context.DeleteAsync(doctor);
    }
}