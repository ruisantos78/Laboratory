using System.Net;
using FluentAssertions;
using RuiSantos.Labs.Api.Contracts;
using RuiSantos.Labs.Api.Tests.Extensions;

namespace RuiSantos.Labs.Api.Tests.Rest.Doctors;

partial class DoctorControllerTests
{
    [Theory(DisplayName = "Should return the doctor's information.")]
    [InlineData("XYZ002")]
    [InlineData("DEF003")]
    [InlineData("PED001")]
    public async Task ShouldReturnDoctorInformation(string license)
    {
        // Act
        var response = await Client.GetAsync($"/Doctor/{license}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert
        var content = await response.Content.GetContractAsync<DoctorContract>(Output);
        content.License.Should().Be(license);
    }

    [Theory(DisplayName = "Should return empty if no records are found for the given license.")]
    [InlineData("ABC123")]
    public async Task ShouldReturnNoContentForNonExistingLicense(string license)
    {
        // Act
        var response = await Client.GetAsync($"/Doctor/{license}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
