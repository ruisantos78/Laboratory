using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using NFluent;
using RuiSantos.Labs.Services.Endpoints.Schemas;
using RuiSantos.Labs.Services.Tests.Fixtures;
using RuiSantos.Labs.Services.Tests.Fixtures.Builders;
using Xunit.Abstractions;

namespace RuiSantos.Labs.Services.Tests.Endpoints.DoctorsEndpoints;

[Collection(nameof(ServiceFixture))]
public class GetDoctorByIdTests(ServiceFixture fixture, ITestOutputHelper output) : EndpointTest(fixture)
{
    [Fact]
    public async Task GetDoctorById_WithValidId_ShouldReturnDoctor()
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
        var response = await Client.GetAsync($"/doctor?id={doctor.Id}");
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
    public async Task GetDoctorById_WithNonExistentId_ShouldReturnNoContent()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"/doctor?id={nonExistentId}");
        await output.HttpResponseAsync(response);

        // Assert
        Check.That(response.StatusCode).IsEqualTo(System.Net.HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task GetDoctorById_WithInvalidId_ShouldReturnBadRequest()
    {
        // Act
        var response = await Client.GetAsync($"/doctor?id=invalid-guid");
        await output.HttpResponseAsync(response);

        // Assert
        Check.That(response.StatusCode).IsEqualTo(System.Net.HttpStatusCode.BadRequest);
        
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Check.That(problem!.Title).IsEqualTo("One or more validation errors occurred.");
        Check.That(problem.Errors["DoctorId"]).ContainsExactly("Doctor Id must be a valid Guid.");
    }
    
    [Fact]
    public async Task GetDoctorById_WithoutId_ShouldReturnBadRequest()
    {
        // Act
        var response = await Client.GetAsync($"/doctor");
        await output.HttpResponseAsync(response);

        // Assert
        Check.That(response.StatusCode).IsEqualTo(System.Net.HttpStatusCode.BadRequest);
    }
}