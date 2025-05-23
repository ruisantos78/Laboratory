using System.Net;
using RuiSantos.Labs.Api.Contracts;
using RuiSantos.Labs.Infrastrucutre.Tests.Extensions;
using RuiSantos.Labs.Data.Dynamodb.Entities;
using static RuiSantos.Labs.Data.Dynamodb.Mappings.MappingConstants;
using RuiSantos.Labs.Core.Models;

namespace RuiSantos.Labs.Infrastrucutre.Tests.Rest.Doctors;

partial class DoctorControllerTests
{
    [Fact(DisplayName = "Should return a success message if the doctor is successfully created.")]
    public async Task ShouldReturnSuccessMessageWhenDoctorIsCreated()
    {
        // Arrange
        DoctorContract request = new Doctor()
        {
            License = "ZZZ123",
            FirstName = "Joe",
            LastName = "Doe",
            Email = "joe.doe@email.net",
            ContactNumbers = [ "1-555-5555" ],
            Specialties = [ "Cardiology" ]
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