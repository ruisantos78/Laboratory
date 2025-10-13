using System.Net.Http.Json;
using System.Text;
using System.Text.Encodings.Web;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.AspNetCore.Mvc;
using NFluent;
using RuiSantos.Labs.Repository.DynamoDb.Entities;
using RuiSantos.Labs.Services.Endpoints.Schemas;
using RuiSantos.Labs.Services.Tests.Fixtures;
using RuiSantos.Labs.Services.Tests.Fixtures.Builders;
using Xunit.Abstractions;

namespace RuiSantos.Labs.Services.Tests.Endpoints.DoctorsEndpoints;

[Collection(nameof(ServiceFixture))]
public class GetDoctorsTests(ServiceFixture fixture, ITestOutputHelper output): EndpointTest(fixture)
{
    [Fact]
    public async Task GetDoctors_WithNoParameters_ShouldReturnAllDoctors()
    {
        // Arrange
        var expected = await CreateDoctorsWithSpecialitiesAsync(Context, 3)
            .ToArrayAsync();
        
        // Act
        var response = await Client.GetAsync("/doctors");
        await output.HttpResponseAsync(response);

        // Assert
        Check.That(response.StatusCode).IsEqualTo(System.Net.HttpStatusCode.OK);
        
        var doctors = await response.Content.ReadFromJsonAsync<GetDoctorsPaginationResponse>();
        Check.That(doctors).IsNotNull();
        Check.That(doctors!.PaginationToken).IsNullOrEmpty();
        Check.That(doctors.Items.Length).IsEqualTo(expected.Length);
        Check.That(doctors.Items).HasFieldsWithSameValues(expected);
    }
    
    [Fact]
    public async Task GetDoctors_WithTakeParameter_ShouldReturnLimitedResults()
    {
        // Arrange
        var expected = await CreateDoctorsWithSpecialitiesAsync(Context, 3)
            .ToArrayAsync();
        
        // Act
        var response = await Client.GetAsync("/doctors?take=1");
        await output.HttpResponseAsync(response);

        // Assert
        Check.That(response.StatusCode).IsEqualTo(System.Net.HttpStatusCode.OK);
        
        var doctors = await response.Content.ReadFromJsonAsync<GetDoctorsPaginationResponse>();
        Check.That(doctors).IsNotNull();
        Check.That(doctors!.PaginationToken).IsNotEmpty();
        Check.That(doctors.Items.Length).IsEqualTo(1);
        Check.That(doctors.Items[0]).HasFieldsWithSameValues(expected[0]);
    }
    
    [Fact]
    public async Task GetDoctors_WithFromParameter_ShouldReturnNextPage()
    {
        // Arrange
        var expected = await CreateDoctorsWithSpecialitiesAsync(Context, 3).ToArrayAsync();
        
        var firstResponse = await Client.GetAsync("/doctors?take=1");
        firstResponse.EnsureSuccessStatusCode();
        
        var firstPage = await firstResponse.Content.ReadFromJsonAsync<GetDoctorsPaginationResponse>();
        var firstPageToken = UrlEncoder.Default.Encode(firstPage!.PaginationToken!);
        
        // Act
        var response = await Client.GetAsync($"/doctors?take=1&from={firstPageToken}");
        await output.HttpResponseAsync(response);

        // Assert
        Check.That(response.StatusCode).IsEqualTo(System.Net.HttpStatusCode.OK);
        
        var doctors = await response.Content.ReadFromJsonAsync<GetDoctorsPaginationResponse>();
        Check.That(doctors).IsNotNull();
        Check.That(doctors!.Items.Length).IsEqualTo(1);
        Check.That(doctors.Items[0]).HasFieldsWithSameValues(expected[1]);
    }

    [Fact]
    public async Task GetDoctors_WithNoData_ShouldReturnNoContent()
    {
        // Act
        var response = await Client.GetAsync("/doctors");
        await output.HttpResponseAsync(response);

        // Assert
        Check.That(response.StatusCode).IsEqualTo(System.Net.HttpStatusCode.NoContent);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task GetDoctors_WithInvalidTakeParameter_ShouldReturnBadRequest(int take)
    {
        // Act
        var response = await Client.GetAsync($"/doctors?take={take}");
        await output.HttpResponseAsync(response);

        // Assert
        Check.That(response.StatusCode).IsEqualTo(System.Net.HttpStatusCode.BadRequest);

        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Check.That(problem!.Title).IsEqualTo("One or more validation errors occurred.");
        Check.That(problem.Errors["Take"]).ContainsExactly("'Take' must be greater than '0'.");        
    }

    [Fact]
    public async Task GetDoctors_WithInvalidFromParameter_ShouldReturnBadRequest()
    {
        // Act
        var response = await Client.GetAsync("/doctors?from=invalid-token");
        await output.HttpResponseAsync(response);

        // Assert
        Check.That(response.StatusCode).IsEqualTo(System.Net.HttpStatusCode.BadRequest);

        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Check.That(problem!.Title).IsEqualTo("One or more validation errors occurred.");
        Check.That(problem.Errors["PaginationToken"]).ContainsExactly("Invalid pagination token format.");
    }
    
    [Fact]
    public async Task GetDoctors_WithInvalidPaginationToken_ShouldReturnBadRequest()
    {
        // Arrange
        var invalidPaginationData = Convert.ToBase64String(Encoding.UTF8.GetBytes("invalid-data"));

        // Act
        var response = await Client.GetAsync($"/doctors?from={invalidPaginationData}");
        await output.HttpResponseAsync(response);

        // Assert
        Check.That(response.StatusCode).IsEqualTo(System.Net.HttpStatusCode.BadRequest);
        
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Check.That(problem!.Title).IsEqualTo("One or more validation errors occurred.");
        Check.That(problem.Errors["PaginationToken"]).ContainsExactly("Invalid DynamoDB pagination token format.");        
    }
    
    private static async IAsyncEnumerable<GetDoctorResponse> CreateDoctorsWithSpecialitiesAsync(DynamoDBContext context, int count)
    {
        var doctorBuilder = new DoctorEntityBuilder();
    
        for (var i = 0; i < count; i++)
        {
            var doctor = await doctorBuilder.WithLicense($"AAA-{i:0000}")
                .BuildAsync(context);

            var specialties = await new DoctorSpecializationEntityBuilder(doctor)
                .BuildAsync(context);
            
            yield return new GetDoctorResponse()
            {
                LicenseNumber = doctor.License,
                FirstName = doctor.FirstName,
                LastName = doctor.LastName,
                Email = doctor.Email,
                ContactNumbers = [..doctor.ContactNumbers],
                Specializations = [..specialties.Select(x => x.Description)]
            };
        }   
    }
}