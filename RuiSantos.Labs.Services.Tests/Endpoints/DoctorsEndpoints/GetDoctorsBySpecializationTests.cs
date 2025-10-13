using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using NFluent;
using RuiSantos.Labs.Services.Endpoints.Schemas;
using RuiSantos.Labs.Services.Tests.Fixtures;
using RuiSantos.Labs.Services.Tests.Fixtures.Builders;
using Xunit.Abstractions;

namespace RuiSantos.Labs.Services.Tests.Endpoints.DoctorsEndpoints;

[Collection(nameof(ServiceFixture))]
public class GetDoctorsBySpecializationTests(ServiceFixture fixture, ITestOutputHelper output) : EndpointTest(fixture)
{
    [Fact]
    public async Task GetDoctorsBySpecialization_WithValidSpecialization_ShouldReturnDoctors()
    {
        // Arrange
        var doctor1 = await new DoctorEntityBuilder()
            .WithLicense("TEST-001")
            .WithFirstName("John")
            .WithLastName("Smith")
            .BuildAsync(Context);

        var doctor2 = await new DoctorEntityBuilder()
            .WithLicense("TEST-002")
            .WithFirstName("Jane")
            .WithLastName("Doe")
            .BuildAsync(Context);

        await new DoctorSpecializationEntityBuilder(doctor1)
            .WithSpecializations("Cardiology")
            .BuildAsync(Context);

        await new DoctorSpecializationEntityBuilder(doctor2)
            .WithSpecializations("Cardiology")
            .BuildAsync(Context);

        // Act
        var response = await Client.GetAsync("/doctors/specialization/cardiology");
        await output.HttpResponseAsync(response);

        // Assert
        Check.That(response.StatusCode).IsEqualTo(System.Net.HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<GetDoctorsCollectionResponse>();
        Check.That(result).IsNotNull();
        Check.That(result!.Items).HasSize(2);
        Check.That(result.Items.Select(x => x.LicenseNumber))
            .Contains("TEST-001", "TEST-002");
    }

    [Fact]
    public async Task GetDoctorsBySpecialization_WithNonExistentSpecialization_ShouldReturnNoContent()
    {
        // Act
        var response = await Client.GetAsync("/doctors/specialization/non_existent");
        await output.HttpResponseAsync(response);

        // Assert
        Check.That(response.StatusCode).IsEqualTo(System.Net.HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task GetDoctorsBySpecialization_WithEmptySpecialization_ShouldReturnNotFound()
    {
        // Act
        var response = await Client.GetAsync("/doctors/specialization/");
        await output.HttpResponseAsync(response);

        // Assert
        Check.That(response.StatusCode).IsEqualTo(System.Net.HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task GetDoctorsBySpecialization_WithMixedSpecializations_ShouldReturnOnlyMatchingDoctors()
    {
        // Arrange
        var cardiologist = await new DoctorEntityBuilder()
            .WithLicense("CARD-001")
            .WithFirstName("Heart")
            .WithLastName("Doctor")
            .BuildAsync(Context);

        var neurologist = await new DoctorEntityBuilder()
            .WithLicense("NEURO-001")
            .WithFirstName("Brain")
            .WithLastName("Doctor")
            .BuildAsync(Context);

        await new DoctorSpecializationEntityBuilder(cardiologist)
            .WithSpecializations("Cardiology")
            .BuildAsync(Context);

        await new DoctorSpecializationEntityBuilder(neurologist)
            .WithSpecializations("Neurology")
            .BuildAsync(Context);

        // Act
        var response = await Client.GetAsync("/doctors/specialization/cardiology");
        await output.HttpResponseAsync(response);

        // Assert
        Check.That(response.StatusCode).IsEqualTo(System.Net.HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<GetDoctorsCollectionResponse>();
        Check.That(result).IsNotNull();
        Check.That(result!.Items).HasSize(1);
        Check.That(result.Items[0].LicenseNumber).IsEqualTo("CARD-001");
        Check.That(result.Items[0].Specializations).Contains("Cardiology");
    }
}