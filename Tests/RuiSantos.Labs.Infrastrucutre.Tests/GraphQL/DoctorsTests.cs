using System.Net;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using RuiSantos.Labs.Data.Dynamodb.Core;
using RuiSantos.Labs.Data.Dynamodb.Entities;
using RuiSantos.Labs.Infrastrucutre.Tests.Extensions;
using RuiSantos.Labs.Infrastrucutre.Tests.Extensions.FluentAssertions;
using RuiSantos.Labs.Infrastrucutre.Tests.Fixtures;

using static RuiSantos.Labs.Data.Dynamodb.Mappings.MappingConstants;

namespace RuiSantos.Labs.Infrastrucutre.Tests.GraphQL;

public class DoctorsTests : IClassFixture<ServiceFixture>
{
    protected record GetDoctorsResult(string License, string FirstName, string LastName, string Email);
    protected record GetDoctorResult(string License, string FirstName, string LastName, string Email, string[] Contacts, string[] Specialties);

    private readonly HttpClient _client;
    private readonly IDynamoDBContext _context;
    private readonly ITestOutputHelper _output;

    public DoctorsTests(ServiceFixture service, ITestOutputHelper output)
    {
        _context = service.GetDynamoDbContext();
        _client = service.GetHttpClient();
        _output = output;
    }

    [Theory(DisplayName = "Get doctor information by license")]
    [InlineData("XYZ002")]
    [InlineData("DEF003")]
    [InlineData("PED001")]
    public async Task GetDoctorInformationByLicense(string license)
    {
        // Arrange
        var doctorEntity = await _context.FindAsync<DoctorEntity>(license, DoctorLicenseIndexName);
        var specialtyEntities = await _context.FindAllAsync<DoctorSpecialtyEntity>(doctorEntity.Id);
        var expected = new GetDoctorResult(
            doctorEntity.License,
            doctorEntity.FirstName,
            doctorEntity.LastName,
            doctorEntity.Email,
            doctorEntity.ContactNumbers.ToArray(),
            specialtyEntities.Select(x => x.Specialty).ToArray()
        );

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
        var response = await _client.PostAsync("graphql", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert
        var result = await response.Content.GetTokenAsync(_output);
        result["data"].Should().HaveChild("doctor")
            .ToObject<GetDoctorResult>()
            .Should()
            .BeEquivalentTo(expected);
    }

    [Theory(DisplayName = "Get doctors with pagination")]
    [InlineData(2, default)]
    [InlineData(2, @"{""DoctorId"":{""S"":""8a6151c7-9122-4f1b-a1e7-85e981c17a14""},""License"":{""S"":""DEF003""}}")]
    public async Task GetDoctorsWithPagination(int limit, string? paginationToken)
    {
        // Arrange
        var search = _context.GetTargetTable<DoctorEntity>().Scan(new ScanOperationConfig
        {
            IndexName = DoctorLicenseIndexName,
            Limit = limit,
            PaginationToken = paginationToken
        });

        var documents = await search.GetNextSetAsync();
        var expected = _context.FromDocuments<DoctorEntity>(documents)
            .Select(x => new GetDoctorsResult(
                x.License,
                x.FirstName,
                x.LastName,
                x.Email
            ));

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

        pagination.Should().HaveChild("paginationToken")
            .Should()
            .Be(Tokens.Encode(search.PaginationToken));

        pagination.Should().HaveChild("doctors")
            .ToObject<List<GetDoctorsResult>>()
            .Should()
            .BeEquivalentTo(expected);
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