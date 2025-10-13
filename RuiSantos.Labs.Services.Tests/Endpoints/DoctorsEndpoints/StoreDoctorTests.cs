using System.Net.Http.Json;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.AspNetCore.Mvc;
using NFluent;
using RuiSantos.Labs.Repository.DynamoDb.Entities;
using RuiSantos.Labs.Services.Endpoints.Schemas;
using RuiSantos.Labs.Services.Tests.Fixtures;
using Xunit.Abstractions;

namespace RuiSantos.Labs.Services.Tests.Endpoints.DoctorsEndpoints;

[Collection(nameof(ServiceFixture))]
public class StoreDoctorTests(ServiceFixture fixture, ITestOutputHelper output) : EndpointTest(fixture)
{
    [Fact]
    public async Task StoreDoctor_WithValidRequest_ShouldCreateData()
    {
        // Arrange
        var request = new StoreDoctorRequest
        {
            LicenseNumber = "ABC-1234",
            FirstName = "Joe",
            LastName = "Doe",
            Email = "joe.doe@email.com",
            ContactNumbers = ["555-1234"],
            Specializations = ["Cardiology"],
            Availability = [
                new StoreDoctorRequestScheduleItem {
                    DayOfWeek = DayOfWeek.Tuesday,
                    TimeSlots = [TimeSpan.FromHours(9), TimeSpan.FromHours(10), TimeSpan.FromHours(11)]
                }
            ]
        };

        // Act
        var response = await Client.PostAsJsonAsync("/doctor", request);
        await output.HttpResponseAsync(response);

        // Assert
        Check.That(response.StatusCode).IsEqualTo(System.Net.HttpStatusCode.NoContent);

        var doctor = await Context
            .QueryAsync<DoctorEntity>(request.LicenseNumber, new QueryConfig { IndexName = DoctorEntity.DoctorLicenceIndexName })
            .GetNextSetAsync()
            .ContinueWith(task => task.Result.FirstOrDefault());

        Check.That(doctor).IsNotNull();
        Check.That(doctor).HasFieldsWithSameValues(new
        {
            License = request.LicenseNumber,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            ContactNumbers = request.ContactNumbers
        });

        string[] specializations = [..
            await Context.QueryAsync<DoctorSpecializationEntity>(doctor!.Id)
                .GetRemainingAsync()
                .ContinueWith(task => task.Result.Select(x => x.Specialization))
        ];
        
        Check.That(specializations).HasSize(request.Specializations.Length);
        Check.That(specializations).ContainsExactly(request.Specializations);
    }

    [Fact]
    public async Task StoreDoctor_WithExistingLicense_ShouldUpdateData()
    {
        // Arrange
        var existingDoctor = new DoctorEntity
        {
            Id = Guid.NewGuid(),
            License = "ABC-1234",
            FirstName = "Existing",
            LastName = "Doctor",
            Email = "existing@email.com",
            ContactNumbers = ["555-0000"]
        };
        await Context.SaveAsync(existingDoctor);

        var request = new StoreDoctorRequest
        {
            LicenseNumber = "ABC-1234",
            FirstName = "Updated",
            LastName = "Name",
            Email = "updated@email.com",
            ContactNumbers = ["555-9999"],
            Specializations = ["Neurology"]
        };

        // Act
        var response = await Client.PostAsJsonAsync("/doctor", request);
        await output.HttpResponseAsync(response);

        // Assert
        Check.That(response.StatusCode).IsEqualTo(System.Net.HttpStatusCode.NoContent);

        var updatedDoctor = await Context
            .QueryAsync<DoctorEntity>(request.LicenseNumber, new QueryConfig { IndexName = DoctorEntity.DoctorLicenceIndexName })
            .GetNextSetAsync()
            .ContinueWith(task => task.Result.FirstOrDefault());

        Check.That(updatedDoctor).IsNotNull();
        Check.That(updatedDoctor).HasFieldsWithSameValues(new
        {
            License = request.LicenseNumber,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            ContactNumbers = request.ContactNumbers
        });
    }

    [Theory]
    [InlineData("LicenseNumber", "", "'License Number' must not be empty.")]
    [InlineData("FirstName", "", "'First Name' must not be empty.")]
    [InlineData("LastName", "", "'Last Name' must not be empty.")]
    [InlineData("Email", "", "'Email' must not be empty.")]
    [InlineData("Email", "invalid-email", "'Email' is not a valid email address.")]
    public async Task StoreDoctor_WithInvalidData_ShouldReturnBadRequest(string propertyName, string value, string message)
    {
        // Arrange
        var request = new StoreDoctorRequest
        {
            LicenseNumber = propertyName.Equals("LicenseNumber") ? value : "ABC-123",
            FirstName = propertyName.Equals("FirstName") ? value : "FirstName",
            LastName = propertyName.Equals("LastName") ? value : "LastName",
            Email = propertyName.Equals("Email") ? value : "email@test.com",
            ContactNumbers = ["555-1234"],
            Specializations = ["Cardiology"]
        };

        // Act
        var response = await Client.PostAsJsonAsync("/doctor", request);
        await output.HttpResponseAsync(response);

        // Assert
        Check.That(response.StatusCode).IsEqualTo(System.Net.HttpStatusCode.BadRequest);
        
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Check.That(problem!.Title).IsEqualTo("One or more validation errors occurred.");
        Check.That(problem.Errors[propertyName]).Contains(message);
    }

    [Fact]
    public async Task StoreDoctor_WithEmptySpecializations_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new StoreDoctorRequest
        {
            LicenseNumber = "ABC-1234",
            FirstName = "Joe",
            LastName = "Doe",
            Email = "joe.doe@email.com",
            ContactNumbers = ["555-1234"],
            Specializations = []
        };

        // Act
        var response = await Client.PostAsJsonAsync("/doctor", request);
        await output.HttpResponseAsync(response);

        // Assert
        Check.That(response.StatusCode).IsEqualTo(System.Net.HttpStatusCode.BadRequest);
                
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Check.That(problem!.Title).IsEqualTo("One or more validation errors occurred.");
        Check.That(problem.Errors["Specializations"]).Contains("'Specializations' must not be empty.");
    }

    [Fact]
    public async Task StoreDoctor_WithEmptyContactNumbers_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new StoreDoctorRequest
        {
            LicenseNumber = "ABC-1234",
            FirstName = "Joe",
            LastName = "Doe",
            Email = "joe.doe@email.com",
            ContactNumbers = [],
            Specializations = ["Cardiology"]
        };

        // Act
        var response = await Client.PostAsJsonAsync("/doctor", request);
        await output.HttpResponseAsync(response);

        // Assert
        Check.That(response.StatusCode).IsEqualTo(System.Net.HttpStatusCode.BadRequest);

        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Check.That(problem!.Title).IsEqualTo("One or more validation errors occurred.");
        Check.That(problem.Errors["ContactNumbers"]).Contains("'Contact Numbers' must not be empty.");
    }

    [Fact]
    public async Task StoreDoctor_WithEmptySpecializationElement_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new StoreDoctorRequest
        {
            LicenseNumber = "ABC-1234",
            FirstName = "Joe",
            LastName = "Doe",
            Email = "joe.doe@email.com",
            ContactNumbers = ["555-1234"],
            Specializations = ["Cardiology", "", "Neurology"]
        };

        // Act
        var response = await Client.PostAsJsonAsync("/doctor", request);
        await output.HttpResponseAsync(response);

        // Assert
        Check.That(response.StatusCode).IsEqualTo(System.Net.HttpStatusCode.BadRequest);

        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Check.That(problem!.Title).IsEqualTo("One or more validation errors occurred.");
        Check.That(problem.Errors["Specializations[1]"]).Contains("'Specializations' must not be empty.");
    }

    [Fact]
    public async Task StoreDoctor_WithEmptyContactElement_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new StoreDoctorRequest
        {
            LicenseNumber = "ABC-1234",
            FirstName = "Joe",
            LastName = "Doe",
            Email = "joe.doe@email.com",
            ContactNumbers = ["555-1234", "", "555-5678"],
            Specializations = ["Cardiology"]
        };

        // Act
        var response = await Client.PostAsJsonAsync("/doctor", request);
        await output.HttpResponseAsync(response);

        // Assert
        Check.That(response.StatusCode).IsEqualTo(System.Net.HttpStatusCode.BadRequest);

        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Check.That(problem!.Title).IsEqualTo("One or more validation errors occurred.");
        Check.That(problem.Errors["ContactNumbers[1]"]).Contains("'Contact Numbers' must not be empty.");
    }

    [Fact]
    public async Task StoreDoctor_WithDuplicateSpecializations_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new StoreDoctorRequest
        {
            LicenseNumber = "ABC-1234",
            FirstName = "Joe",
            LastName = "Doe",
            Email = "joe.doe@email.com",
            ContactNumbers = ["555-1234"],
            Specializations = ["Cardiology", "Neurology", "Cardiology"]
        };

        // Act
        var response = await Client.PostAsJsonAsync("/doctor", request);
        await output.HttpResponseAsync(response);

        // Assert
        Check.That(response.StatusCode).IsEqualTo(System.Net.HttpStatusCode.BadRequest);

        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Check.That(problem!.Title).IsEqualTo("One or more validation errors occurred.");
        Check.That(problem.Errors["Specializations"]).Contains("Specializations must be unique.");
    }

    [Fact]
    public async Task StoreDoctor_WithDuplicateContactNumbers_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new StoreDoctorRequest
        {
            LicenseNumber = "ABC-1234",
            FirstName = "Joe",
            LastName = "Doe",
            Email = "joe.doe@email.com",
            ContactNumbers = ["555-1234", "555-5678", "555-1234"],
            Specializations = ["Cardiology"]
        };

        // Act
        var response = await Client.PostAsJsonAsync("/doctor", request);
        await output.HttpResponseAsync(response);

        // Assert
        Check.That(response.StatusCode).IsEqualTo(System.Net.HttpStatusCode.BadRequest);

        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Check.That(problem!.Title).IsEqualTo("One or more validation errors occurred.");
        Check.That(problem.Errors["ContactNumbers"]).Contains("Contact numbers must be unique.");
    }

    [Fact]
    public async Task StoreDoctor_WithNullRequest_ShouldReturnBadRequest()
    {
        // Act
        var response = await Client.PostAsJsonAsync("/doctor", (StoreDoctorRequest?)null);
        await output.HttpResponseAsync(response);

        // Assert
        Check.That(response.StatusCode).IsEqualTo(System.Net.HttpStatusCode.BadRequest);
    }
}