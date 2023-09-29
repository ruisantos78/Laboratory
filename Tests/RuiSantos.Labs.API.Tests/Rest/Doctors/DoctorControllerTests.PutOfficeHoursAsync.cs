using System.Net;
using FluentAssertions;
using RuiSantos.Labs.Data.Dynamodb.Entities;

using static RuiSantos.Labs.Data.Dynamodb.Mappings.ClassMapConstants;

namespace RuiSantos.Labs.Api.Tests.Rest;

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
            .SelectMany(i => new[] {
                    TimeSpan.FromHours(i),
                    TimeSpan.FromMinutes(i * 60 + 30) }
            ).ToArray();

        // Act
        var response = await client.PutAsync($"/Doctor/{license}/OfficeHours/{week}", hours, output);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var doctor = await context.FindAsync<DoctorEntity>(license, DoctorLicenseIndexName);
        doctor.Availability.Should().HaveCount(1);
        doctor.Availability.Should().ContainSingle(i => i.Week == week && i.Hours.SequenceEqual(hours));

        // Teardown
        doctor.Availability.Clear();
        await context.SaveAsync(doctor);
    }
}