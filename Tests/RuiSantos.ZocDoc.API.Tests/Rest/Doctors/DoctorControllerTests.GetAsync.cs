using System.Net;
using FluentAssertions;
using RuiSantos.ZocDoc.Api.Contracts;

namespace RuiSantos.ZocDoc.API.Tests.Rest;

partial class DoctorControllerTests
{
    [Theory(DisplayName = "Should returns the doctor's information.")]
    [InlineData("XYZ002")]
    [InlineData("DEF003")]
    [InlineData("PED001")]
    public async Task GetAsync_ReturnsOk_WhenLicenseIsValid(string license)
    {
        // Act
        var response = await client.GetAsync($"/Doctor/{license}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert
        var content = await response.Content.As<DoctorContract>(output);
        content.License.Should().Be(license);        
    }

    [Theory(DisplayName = "Should return empty if no records are found for the given license.")]
    [InlineData("ABC123")]
    public async Task GetAsync_ReturnNoContent_WhenLicenseNotExists(string license)
    {
        // Act & Assert
        var response = await client.GetAsync($"/Doctor/{license}");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}