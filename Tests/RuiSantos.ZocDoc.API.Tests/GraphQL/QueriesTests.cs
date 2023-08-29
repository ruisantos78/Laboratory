using System.Net;
using Amazon.DynamoDBv2.DataModel;
using FluentAssertions;
using RuiSantos.ZocDoc.API.Tests.Fixtures;
using Xunit.Abstractions;

namespace RuiSantos.ZocDoc.API.Tests.GraphQL;

public class QueriesTests : IClassFixture<ServiceFixture>
{
    private readonly HttpClient client;
    private readonly IDynamoDBContext context;
    private readonly ITestOutputHelper output;

    public QueriesTests(ServiceFixture service, ITestOutputHelper output)
    {
        this.client = service.GetClient();
        this.context = service.GetContext();       
        this.output = output;
    }

    [Theory(DisplayName = "Should returns the doctor's information.")]
    [InlineData("XYZ002")]
    [InlineData("DEF003")]
    [InlineData("PED001")]
    public async Task GetDoctor(string license)
    {
        // Arrange
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
                        specialties
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
        doctor["license"].Should().Be(license);
    }
}
