using System.Net;
using FluentAssertions;
using RuiSantos.ZocDoc.Api.Contracts;
using RuiSantos.ZocDoc.Data.Dynamodb.Entities;

using static RuiSantos.ZocDoc.Data.Dynamodb.Mappings.ClassMapConstants;

namespace RuiSantos.ZocDoc.API.Tests.Rest;

partial class DoctorControllerTests
{
    [Fact(DisplayName = "Should returns a success message if the doctor is successfully created.")]
    public async Task PostAsync_ReturnsCreated_WhenDoctorIsCreated()
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
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        // Assert
        var doctor = await context.FindAsync<DoctorDto>(DoctorLicenseIndexName, "ZZZ123");
        doctor.Should().NotBeNull();
        doctor!.FirstName.Should().Be("Joe");
        doctor!.LastName.Should().Be("Doe");
        doctor!.Email.Should().Be("joe.doe@email.net");
        doctor!.ContactNumbers.Should().HaveCount(1).And.ContainSingle("1-555-5555");
        
        var specialties = await context.QueryAsync<DoctorSpecialtyDto>(doctor.Id)
            .GetRemainingAsync();

        specialties.Should().NotBeNullOrEmpty();
        specialties.Should().HaveCount(1).And.ContainSingle(x => x.Specialty == "Cardiology");

        // Teardown
        await context.DeleteAsync(doctor);
    }

    [Theory(DisplayName = "Should returns a success message if the office hours are successfully set.")]
    [InlineData("CAR002", DayOfWeek.Monday)]
    [InlineData("CAR002", DayOfWeek.Wednesday)]
    [InlineData("CAR002", DayOfWeek.Friday)]
    public async Task PutOfficeHoursAsync_ReturnsOk_WhenOfficeHoursAreUpdated(string license, DayOfWeek week)
    {
        // Arrange
        var hours = Enumerable.Range(9, 4)
            .SelectMany(i => new[] {
                TimeSpan.FromHours(i),
                TimeSpan.FromMinutes(i * 60 + 30) }
            ).ToArray();

        // Act
        var response = await client.PutAsync($"/Doctor/{license}/OfficeHours/{week}", hours, output);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert
        var doctor = await context.FindAsync<DoctorDto>(DoctorLicenseIndexName, "CAR002");
        doctor.Should().NotBeNull();
        doctor!.Availability.Should().HaveCount(1);
        doctor.Availability.Should().ContainSingle(i => i.Week == week && i.Hours.SequenceEqual(hours));

        // Teardown
        doctor.Availability.Clear();
        await context.SaveAsync(doctor);
    }
}
