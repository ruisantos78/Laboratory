using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using NFluent;
using RuiSantos.Labs.Services.Endpoints.Schemas;
using RuiSantos.Labs.Services.Tests.Fixtures;
using RuiSantos.Labs.Services.Tests.Fixtures.Builders;
using Xunit.Abstractions;

namespace RuiSantos.Labs.Services.Tests.Endpoints.DoctorsEndpoints;

[Collection(nameof(ServiceFixture))]
public class GetDoctorByLicenseTests(ServiceFixture fixture, ITestOutputHelper output) : EndpointTest(fixture)
{
    [Fact]
    public async Task GetDoctorByLicense_WithValidLicense_ShouldReturnDoctor()
    {
        // Arrange
        var doctor = await new DoctorBuilder()
            .WithLicense("TEST-001")
            .WithFirstName("John")
            .WithLastName("Smith")
            .WithEmail("john.smith@test.com")
            .BuildAsync(Context);

        await new DoctorSpecializationBuilder()
            .WithDoctor(doctor)
            .WithSpecializations("Cardiology")
            .BuildAsync(Context);

        // Act
        var response = await Client.GetAsync($"/doctor/{doctor.License}");
        await output.HttpResponseAsync(response);

        // Assert
        Check.That(response.StatusCode).IsEqualTo(System.Net.HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<GetDoctorResponse>();
        Check.That(result).IsNotNull();
        Check.That(result!.LicenseNumber).IsEqualTo(doctor.License);
        Check.That(result.FirstName).IsEqualTo(doctor.FirstName);
        Check.That(result.LastName).IsEqualTo(doctor.LastName);
        Check.That(result.Email).IsEqualTo(doctor.Email);
        Check.That(result.Specializations).Contains("Cardiology");
    }

    [Fact]
    public async Task GetDoctorByLicense_WithNonExistentLicense_ShouldReturnNoContent()
    {
        // Act
        var response = await Client.GetAsync("/doctor/NON-EXISTENT");
        await output.HttpResponseAsync(response);

        // Assert
        Check.That(response.StatusCode).IsEqualTo(System.Net.HttpStatusCode.NoContent);
    }
}