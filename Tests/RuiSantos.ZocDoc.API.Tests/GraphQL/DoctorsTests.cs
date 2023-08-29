using System.Net;
using Amazon.DynamoDBv2.DataModel;
using FluentAssertions;
using RuiSantos.ZocDoc.API.Tests.Fixtures;
using RuiSantos.ZocDoc.Data.Dynamodb.Entities;
using Xunit.Abstractions;
using static RuiSantos.ZocDoc.Data.Dynamodb.Mappings.ClassMapConstants;

namespace RuiSantos.ZocDoc.API.Tests.GraphQL;

[Collection(nameof(ServiceCollectionFixture))]
public class DoctorsTests : IClassFixture<ServiceFixture>
{
    protected readonly HttpClient client;
    protected readonly IDynamoDBContext context;
    protected readonly ITestOutputHelper output;

    public DoctorsTests(ServiceFixture service, ITestOutputHelper output)
    {
        this.context = service.GetContext();
        this.client = service.GetClient();
        this.output = output;
    }

    [Theory(DisplayName = "Should returns the doctor's information.")]
    [InlineData("XYZ002")]
    [InlineData("DEF003")]
    [InlineData("PED001")]
    public async Task ShouldReturnDoctorInformation(string license)
    {
        // Arrange
        var expected = await context.FirstAsync<DoctorDto>(DoctorLicenseIndexName, license);

        var request = new
        {
            query = """
                    query GetDoctor($license: String!) {
                        doctor(license: $license) {
                            license
                            firstName
                            lastName
                            email
                            contacts
                        }
                    }
                    """,
            variables = new
            {
                license
            }
        };

        // Act
        var response = await client.PostAsync("graphql", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert
        var result = await response.Content.GetTokenAsync();

        var doctor = result["data"].Should().HaveChild("doctor");
        doctor["license"].Should().Be(expected.License);
        doctor["firstName"].Should().Be(expected.FirstName);
        doctor["lastName"].Should().Be(expected.LastName);
        doctor["email"].Should().Be(expected.Email);
        doctor["contacts"].Should().ContainsAll(expected.ContactNumbers);
    }
}
