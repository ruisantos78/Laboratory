using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json.Linq;
using RuiSantos.ZocDoc.API.Tests.Fixtures;

namespace RuiSantos.ZocDoc.API.Tests;

public class DoctorControllerTests: IClassFixture<ServiceFixture>
{
    private readonly TestServer host;

    public DoctorControllerTests(ServiceFixture serviceFixture)
    {
        this.host = serviceFixture.Server;
    }

    [Theory]
    [InlineData("XYZ002")]
    [InlineData("DEF003")]
    [InlineData("PED001")]
    public async Task GetAsync_ShouldReturnOk_WithValidLicenseNumber(string medicalLicense)    
    {
        // Act
        var response = await host.CreateRequest($"/Doctor/{medicalLicense}").GetAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert               
        var context = JToken.Parse(await response.Content.ReadAsStringAsync());        
        context["license"]?.Value<String>().Should().Be(medicalLicense);        
    }
}
