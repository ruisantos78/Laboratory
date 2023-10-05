using System.Net;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using RuiSantos.Labs.Data.Dynamodb.Core;
using RuiSantos.Labs.Data.Dynamodb.Entities;
using RuiSantos.Labs.GraphQL.Services;
using RuiSantos.Labs.Infrastrucutre.Tests.Extensions;
using RuiSantos.Labs.Infrastrucutre.Tests.Extensions.FluentAssertions;
using RuiSantos.Labs.Infrastrucutre.Tests.Fixtures;
using Xunit.Abstractions;
using static RuiSantos.Labs.Data.Dynamodb.Mappings.MappingConstants;

namespace RuiSantos.Labs.Infrastrucutre.Tests.GraphQL;

[Collection(nameof(ServiceCollectionFixture))]
public class DoctorsTests : IClassFixture<ServiceFixture>
{
    private readonly HttpClient _client;
    private readonly IDynamoDBContext _context;
    private readonly ITestOutputHelper _output;
    private readonly ISecurity _security = new Security();

    public DoctorsTests(ServiceFixture service, ITestOutputHelper output)
    {
        _context = service.GetContext();
        _client = service.GetClient();
        _output = output;
    }

    [Theory(DisplayName = "Get doctor information by license")]
    [InlineData("d6c9f315-0e35-4d5b-b25e-61a61c92d9c9")]
    [InlineData("8a6151c7-9122-4f1b-a1e7-85e981c17a14")]
    [InlineData("fa626a8e-03a5-4fe1-a910-b3803bed256c")]
    public async Task GetDoctorInformationByLicense(string uuid)
    {
        // Arrange
        var doctorId = Guid.Parse(uuid);
        var expected = await _context.LoadAsync<DoctorEntity>(doctorId);
        var expectedSpecialties = await _context.FindAllAsync<DoctorSpecialtyEntity>(expected.Id);

        var request = new
        {
            query = """
                    query GetDoctor($id: String!) {
                        doctor(id: $id) {
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
                id = _security.Encode(doctorId)
            }
        };

        // Act
        var response = await _client.PostAsync("graphql", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert
        var result = await response.Content.GetTokenAsync(_output);

        var doctor = result["data"].Should().HaveChild("doctor");
        doctor["license"].Should().Be(expected.License);
        doctor["firstName"].Should().Be(expected.FirstName);
        doctor["lastName"].Should().Be(expected.LastName);
        doctor["email"].Should().Be(expected.Email);
        doctor["contacts"].Should().BeEquivalentTo(expected.ContactNumbers);
        doctor["specialties"].Should().BeEquivalentTo(expectedSpecialties.Select(ds => ds.Specialty));
    }

    [Theory(DisplayName = "Get doctors with pagination")]
    [InlineData(2, default)]
    [InlineData(2, @"{""DoctorId"":{""S"":""8a6151c7-9122-4f1b-a1e7-85e981c17a14""},""License"":{""S"":""DEF003""}}")]
    public async Task GetDoctorsWithPagination(int limit, string? paginationToken)
    {
        // Arrange
        var search = _context.GetTargetTable<DoctorEntity>().Scan(new ScanOperationConfig()
        {
            IndexName = DoctorLicenseIndexName,
            Limit = limit,
            PaginationToken = paginationToken
        });

        var documents = await search.GetNextSetAsync();

        var request = new
        { 
            query = """
                    query GetDoctors($page: PaginationInput!) {
                      doctors(page: $page) {
                        doctors {
                          license
                          firstName
                          lastName
                          email
                        }
                        paginationToken
                      }
                    }
                    """,
            variables = new
            {
                page = new
                {
                    take = 2,
                    token = Tokens.Encode(paginationToken)
                }
            }
        };

        // Act
        var response = await _client.PostAsync("graphql", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert
        var result = await response.Content.GetTokenAsync();
        var pagination = result["data"].Should().HaveChild("doctors");

        pagination["paginationToken"].Should().Be(Tokens.Encode(search.PaginationToken));
        pagination["doctors"]!.Values<string>("license").Should().BeEquivalentTo(documents.Select(x => x["License"].AsString()));
        pagination["doctors"]!.Values<string>("firstName").Should().BeEquivalentTo(documents.Select(x => x["FirstName"].AsString()));
        pagination["doctors"]!.Values<string>("lastName").Should().BeEquivalentTo(documents.Select(x => x["LastName"].AsString()));
        pagination["doctors"]!.Values<string>("email").Should().BeEquivalentTo(documents.Select(x => x["Email"].AsString()));
    }

    [Fact(DisplayName = "Create a new doctor")]
    public async Task CreateNewDoctor()
    {
        // Arrange
        var request = new
        {
            query = """
                    mutation SetDoctor($input: SetDoctorInput!) {
                        setDoctor(input: $input) {
                            doctor {
                                license
                                firstName
                                lastName
                                email
                                contacts
                                specialties
                            }
                        }
                    }
                    """,
            variables = new
            {
                input = new
                {
                    doctor = new
                    {
                        license = "ABC456",
                        firstName = "John",
                        lastName = "Doe",
                        email = "john.doe@example.com",
                        contacts = new[] { "123-456-7890" },
                        specialties = new[] { "Cardiology", "Pediatrics" }
                    }
                }
            }
        };

        // Act
        var response = await _client.PostAsync("graphql", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert
        var doctor = await _context.FindAsync<DoctorEntity>("ABC456", DoctorLicenseIndexName);
        doctor.License.Should().Be("ABC456");
        doctor.FirstName.Should().Be("John");
        doctor.LastName.Should().Be("Doe");
        doctor.Email.Should().Be("john.doe@example.com");
        doctor.ContactNumbers.Should().AllBe("123-456-7890");

        var specialties = await _context.FindAllAsync<DoctorSpecialtyEntity>(doctor.Id);
        specialties.Should().HaveCount(2);
        specialties.Select(ds => ds.Specialty).Should().BeEquivalentTo("Cardiology", "Pediatrics");
    }
}