using System.Net;
using Amazon.DynamoDBv2.DataModel;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using RuiSantos.Labs.Api.Tests.Fixtures;
using RuiSantos.Labs.Data.Dynamodb.Entities;
using Xunit.Abstractions;

namespace RuiSantos.Labs.Api.Tests;

[Collection(nameof(ServiceCollectionFixture))]
public class MedicalSpecialtiesTests : IClassFixture<ServiceFixture>
{
    protected readonly HttpClient client;
    protected readonly IDynamoDBContext context;
    protected readonly ITestOutputHelper output;

    public MedicalSpecialtiesTests(ServiceFixture service, ITestOutputHelper output)
    {
        this.context = service.GetContext();
        this.client = service.GetClient();
        this.output = output;
    }

    [Fact(DisplayName = "Get all medical specialties")]
    public async Task GetAllMedicalSpecialties()
    {
        // Arrange
        var expected = (await context.FindAllAsync<DictionaryDto>("specialties"))
            .Select(x => x.Value)
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
        var response = await client.PostAsync("graphql", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert
        var result = await response.Content.GetTokenAsync();
        var specialties = result["data"].Should().HaveChild("specialties").AsJEnumerable();
        specialties.Values<string>("description").Should().BeEquivalentTo(expected);
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
        var response = await client.PostAsync("graphql", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert
        var specialties = await context.FindAllAsync<DictionaryDto>("specialties");
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
        await context.SaveAsync(new DictionaryDto
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
                input = new { 
                    description = "Oncology" 
                }
            }
        };

        // Act
        var response = await client.PostAsync("graphql", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert
        var specialties = await context.FindAllAsync<DictionaryDto>("specialties");
        specialties.Should().OnlyHaveUniqueItems().And.NotContain(x => x.Value == "Oncology");
    }
}
