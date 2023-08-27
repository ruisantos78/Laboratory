using System.Net;
using FluentAssertions;
using RuiSantos.ZocDoc.Data.Dynamodb.Entities;

using static RuiSantos.ZocDoc.Data.Dynamodb.Mappings.ClassMapConstants;

namespace RuiSantos.ZocDoc.API.Tests.Rest;

partial class DoctorControllerTests
{
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
