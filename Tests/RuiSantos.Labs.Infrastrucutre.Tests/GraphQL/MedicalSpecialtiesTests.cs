using System.Net;
using Amazon.DynamoDBv2.DataModel;
using RuiSantos.Labs.Infrastrucutre.Tests.Extensions;
using RuiSantos.Labs.Infrastrucutre.Tests.Extensions.FluentAssertions;
using RuiSantos.Labs.Infrastrucutre.Tests.Fixtures;
using RuiSantos.Labs.Data.Dynamodb.Entities;

namespace RuiSantos.Labs.Infrastrucutre.Tests.GraphQL;

public class MedicalSpecialtiesTests : IClassFixture<ServiceFixture>
{
    protected record GetMedicalSpecialtiesResult(string Description);

    private readonly HttpClient _client;
    private readonly IDynamoDBContext _context;
    private readonly ITestOutputHelper _output;

    public MedicalSpecialtiesTests(ServiceFixture service, ITestOutputHelper output)
    {
        _context = service.GetDynamoDbContext();
        _client = service.GetHttpClient();
        _output = output;
    }

    [Fact(DisplayName = "Get all medical specialties")]
    public async Task GetAllMedicalSpecialties()
    {
        // Arrange
        var expected = (await _context.FindAllAsync<DictionaryEntity>("specialties"))
            .Select(x => new GetMedicalSpecialtiesResult(x.Value))
            .ToArray();

        var request = new
        {
            query = """
                    query GetMedicalSpecialties() {
                        specialties() {
                            description
                        }
                    }
                    """
        };

        // Act
        var response = await _client.PostAsync("graphql", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert
        var result = await response.Content.GetTokenAsync(_output);

        result["data"].Should().HaveChild("specialties")
            .ToObject<GetMedicalSpecialtiesResult[]>()
            .Should()
            .BeEquivalentTo(expected);
    }

    [Fact(DisplayName = "Update medical specialties list")]
    public async Task UpdateMedicalSpecialtiesList()
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
        var response = await _client.PostAsync("graphql", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert
        var specialties = await _context.FindAllAsync<DictionaryEntity>("specialties");
        specialties.Select(x => x.Value).Should().OnlyHaveUniqueItems().And.Contain(new[]
        {
            "Gastroenterology",
            "Endocrinology",
            "Nephrology",
            "Rheumatology",
            "Oncology"
        });
    }

    [Fact(DisplayName = "Remove medical specialties list")]
    public async Task RemoveMedicalSpecialtiesList()
    {
        // Arrange
        await _context.SaveAsync(new DictionaryEntity
        {
            Source = "specialties",
            Value = "Oncology"
        });

        var request = new
        {
            query = """
                    mutation RemoveSpecialties($input: RemoveSpecialtiesInput!) {
                        removeSpecialties(input: $input) {
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
                    description = "Oncology"
                }
            }
        };

        // Act
        var response = await _client.PostAsync("graphql", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert
        var specialties = await _context.FindAllAsync<DictionaryEntity>("specialties");
        specialties.Should().OnlyHaveUniqueItems().And.NotContain(x => x.Value == "Oncology");
    }
}
