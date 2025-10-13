using System.Net.Http.Json;
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
public class GetDoctorsTests(ServiceFixture fixture, ITestOutputHelper output) : IAsyncLifetime
{
    private DynamoDBContext? _context;
    private HttpClient? _client;

    public Task InitializeAsync()
    {
        _context = fixture.CreateDynamoDbContext();
        _client = fixture.CreateHttpClient();
        
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        if (_context != null)
            await CleanupDoctorsWithSpecialtiesAsync(_context);
        
        _context?.Dispose();
        _client?.Dispose();
    }

    [Fact]
    public async Task GetDoctors_WithNoParameters_ShouldReturnAllDoctors()
    {
        // Arrange
        var expected = await CreateDoctorsWithSpecialitiesAsync(_context!, 3)
            .ToArrayAsync();
        
        // Act
        var response = await _client!.GetAsync("/doctors");
        await output.HttpResponseAsync(response);

        // Assert
        Check.That(response.StatusCode).IsEqualTo(System.Net.HttpStatusCode.OK);
        
        var doctors = await response.Content.ReadFromJsonAsync<GetDoctorsResponse>();
        Check.That(doctors).IsNotNull();
        Check.That(doctors!.PaginationToken).IsNull();
        Check.That(doctors.Items.Length).IsEqualTo(expected.Length);
        Check.That(doctors.Items).HasFieldsWithSameValues(expected);
    }
    
    [Fact]
    public async Task GetDoctors_WithTakeParameter_ShouldReturnLimitedResults()
    {
        // Arrange
        var expected = await CreateDoctorsWithSpecialitiesAsync(_context!, 3)
            .ToArrayAsync();
        
        // Act
        var response = await _client!.GetAsync("/doctors?take=1");
        await output.HttpResponseAsync(response);

        // Assert
        Check.That(response.StatusCode).IsEqualTo(System.Net.HttpStatusCode.OK);
        
        var doctors = await response.Content.ReadFromJsonAsync<GetDoctorsResponse>();
        Check.That(doctors).IsNotNull();
        Check.That(doctors!.PaginationToken).IsNotEmpty();
        Check.That(doctors.Items.Length).IsEqualTo(1);
        Check.That(doctors.Items[0]).HasFieldsWithSameValues(expected[0]);
    }
    
    [Fact]
    public async Task GetDoctors_WithFromParameter_ShouldReturnNextPage()
    {
        // Arrange
        var expected = await CreateDoctorsWithSpecialitiesAsync(_context!, 3).ToArrayAsync();
        
        var firstResponse = await _client!.GetAsync("/doctors?take=1");
        firstResponse.EnsureSuccessStatusCode();
        var firstPage = await firstResponse.Content.ReadFromJsonAsync<GetDoctorsResponse>();
        
        // Act
        var response = await _client.GetAsync($"/doctors?take=1&from={firstPage?.PaginationToken}");
        await output.HttpResponseAsync(response);

        // Assert
        Check.That(response.StatusCode).IsEqualTo(System.Net.HttpStatusCode.OK);
        
        var doctors = await response.Content.ReadFromJsonAsync<GetDoctorsResponse>();
        Check.That(doctors).IsNotNull();
        Check.That(doctors!.Items.Length).IsEqualTo(1);
        Check.That(doctors.Items[0]).HasFieldsWithSameValues(expected[1]);
    }

    [Fact]
    public async Task GetDoctors_WithNoData_ShouldReturnNoContent()
    {
        // Act
        var response = await _client!.GetAsync("/doctors");
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
        var response = await _client!.GetAsync($"/doctors?take={take}");
        await output.HttpResponseAsync(response);

        // Assert
        Check.That(response.StatusCode).IsEqualTo(System.Net.HttpStatusCode.BadRequest);

        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Check.That(problem!.Title).IsEqualTo("One or more validation errors occurred.");
        Check.That(problem.Errors).HasFieldsWithSameValues(new Dictionary<string, string[]>()
        {
            { "Take", ["'Take' must be greater than '0'."] }
        });
    }

    [Fact]
    public async Task GetDoctors_WithInvalidFromParameter_ShouldReturnBadRequest()
    {
        // Act
        var response = await _client!.GetAsync("/doctors?from=invalid-token");
        await output.HttpResponseAsync(response);

        // Assert
        Check.That(response.StatusCode).IsEqualTo(System.Net.HttpStatusCode.BadRequest);
        
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Check.That(problem!.Title).IsEqualTo("One or more validation errors occurred.");
        Check.That(problem.Errors).HasFieldsWithSameValues(new Dictionary<string, string[]>()
        {
            { "PaginationToken", ["Invalid pagination token format"] }
        });
    }
    
    private static async IAsyncEnumerable<GetDoctorsResponseItem> CreateDoctorsWithSpecialitiesAsync(DynamoDBContext context, int count)
    {
        var doctorBuilder = new DoctorBuilder();
        var doctorSpecialtyBuilder = new DoctorSpecializationBuilder();
        for (var i = 0; i < count; i++)
        {
            var doctor = await doctorBuilder.WithLicense($"AAA-{i:0000}")
                .BuildAsync(context);

            var specialties = await doctorSpecialtyBuilder.WithDoctor(doctor)
                .BuildAsync(context);
            
            yield return new GetDoctorsResponseItem()
            {
                LicenseNumber = doctor.License,
                ContactNumbers = [..doctor.ContactNumbers],
                FirstName = doctor.FirstName,
                LastName = doctor.LastName,
                Email = doctor.Email,
                Specializations = [..specialties.Select(x => x.Specialization)]
            };
        }   
    }

    private static async Task CleanupDoctorsWithSpecialtiesAsync(DynamoDBContext context)
    {
        await Task.WhenAll(
            context.DeleteAllAsync<DoctorEntity>(),
            context.DeleteAllAsync<DoctorSpecializationEntity>()
        );
    }
}