using System.Net;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using RuiSantos.ZocDoc.API.Tests.Fixtures;

namespace RuiSantos.ZocDoc.API.Tests;

public class DoctorControllerTests: IClassFixture<ServiceFixture>
{
    private readonly HttpClient client;

    public DoctorControllerTests(ServiceFixture serviceFixture)
    {
        this.client = serviceFixture.Client;
    }

    [Theory]
    [InlineData("XYZ002")]
    [InlineData("DEF003")]
    [InlineData("PED001")]
    public async Task GetAsync_ShouldReturnOk_WithValidLicenseNumber(string medicalLicense)    
    {
        // Act
        var response = await client.GetAsync($"/Doctor/{medicalLicense}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert               
        var context = JToken.Parse(await response.Content.ReadAsStringAsync());        
        context["license"]?.Value<String>().Should().Be(medicalLicense);        
    }
}
