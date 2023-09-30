using System.Net;
using FluentAssertions;
using RuiSantos.Labs.Api.Tests.Extensions;
using RuiSantos.Labs.Data.Dynamodb.Entities;

using static RuiSantos.Labs.Data.Dynamodb.Mappings.MappingConstants;


namespace RuiSantos.Labs.Api.Tests.Rest.Doctors;

partial class DoctorControllerTests
{
    [Theory(DisplayName = "Should return a success message if the office hours are successfully set.")]
    [InlineData("CAR002", DayOfWeek.Monday)]
    [InlineData("CAR002", DayOfWeek.Wednesday)]
    [InlineData("CAR002", DayOfWeek.Friday)]
    public async Task ShouldReturnSuccessMessageWhenOfficeHoursAreUpdated(string license, DayOfWeek week)
    {
        // Arrange
        var hours = Enumerable.Range(9, 4)
                .SelectMany(i => new[] { TimeSpan.FromHours(i), TimeSpan.FromMinutes(i * 60 + 30) })
                .ToArray();

        // Act
        var response = await Client.PutAsync($"/Doctor/{license}/OfficeHours/{week}", hours, Output);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var doctor = await Context.FindAsync<DoctorEntity>(license, DoctorLicenseIndexName);
        doctor.Availability.Should().HaveCount(1);
        doctor.Availability.Should().ContainSingle(i => i.Week == week && i.Hours.SequenceEqual(hours));

        // Teardown
        doctor.Availability.Clear();
        await Context.SaveAsync(doctor);
    }
}