using System.Net;
using Amazon.DynamoDBv2.DataModel;
using FluentAssertions;
using Newtonsoft.Json.Linq;
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
        var expected = await context.FindAsync<DoctorDto>(DoctorLicenseIndexName, license);
        var expectedSpecialties = await context.QueryAsync<DoctorSpecialtyDto>(expected.Id).GetRemainingAsync();

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
        doctor["license"].Should().Be(expected.License);
        doctor["firstName"].Should().Be(expected.FirstName);
        doctor["lastName"].Should().Be(expected.LastName);
        doctor["email"].Should().Be(expected.Email);
        doctor["contacts"].Should().BeEquivalentTo(expected.ContactNumbers);
        doctor["specialties"].Should().BeEquivalentTo(expectedSpecialties.Select(ds => ds.Specialty));
    }

    [Fact(DisplayName = "Should create a new doctor.")]
    public async Task ShouldCreateNewDoctor()
    {
        // Arrange
        var request = new
        {
            query = """
                    mutation AddDoctor($input: SetDoctorInput!) {
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
        var response = await client.PostAsync("graphql", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert
        var doctor = await context.FindAsync<DoctorDto>(DoctorLicenseIndexName, "ABC456");
        doctor.License.Should().Be("ABC456");
        doctor.FirstName.Should().Be("John");
        doctor.LastName.Should().Be("Doe");
        doctor.Email.Should().Be("john.doe@example.com");
        doctor.ContactNumbers.Should().AllBe("123-456-7890");

        var specialties = await context.QueryAsync<DoctorSpecialtyDto>(doctor.Id).GetRemainingAsync();
        specialties.Should().HaveCount(2);
        specialties.Select(ds => ds.Specialty).Should().BeEquivalentTo(new[] { "Cardiology", "Pediatrics" });
    }

    [Fact(DisplayName = "Should returns all medical specialties")]
    public async Task ShouldReturnAllMedicalSpecialties()
    {
        // Arrange
        var expected = await context.LoadAsync<DictionaryDto>("specialties");

        var request = new
        {
            query = """
                    query GetSpecialties {
                        specialties {
                            description 
                        }
                    }
                    """
        };

        // Act
        var response = await client.PostAsync("graphql", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert
        var result = await response.Content.GetTokenAsync();
        var specialties = result["data"].Should().HaveChild("specialties").AsJEnumerable();
        specialties.Values<string>("description").Should().BeEquivalentTo(expected.Values);
    }

    [Fact(DisplayName = "Should update the medical specialties list")]
    public async Task ShouldUpdateMedicalSpecialtiesList()
    {
        // Arrange
        var request = new
        {
            query = """
                    mutation AddSpecialties($input: AddSpecialtiesInput!) {
                        addSpecialties(input: $input) {
                            specialties {
                                description 
                            }
                        }
                    }
                    """,
            variables = new
            {
                input = new
                {
                    descriptions = new[] 
                    {
                        "Gastroenterology",
                        "Endocrinology",
                        "Nephrology",
                        "Rheumatology",
                        "Oncology"
                    }
                }
            }
        };

        // Act
        var response = await client.PostAsync("graphql", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert
        var specialties = await context.LoadAsync<DictionaryDto>("specialties");
        specialties.Values.Should().OnlyHaveUniqueItems().And.Contain(new[]
         {
            "Gastroenterology",
            "Endocrinology",
            "Nephrology",
            "Rheumatology",
            "Oncology"
        });
    }
}
