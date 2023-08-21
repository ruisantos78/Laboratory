using Microsoft.AspNetCore.Mvc.Testing;
using RuiSantos.ZocDoc.API.Tests.Containers;

namespace RuiSantos.ZocDoc.API.Tests.Fixtures;

public class ServiceFixture : IAsyncLifetime
{
    private readonly DynamoDbContainer dynamoDbContainer;
    private readonly WebApplicationFactory<Program> factory;

    public ServiceFixture()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_URLS", "http://+:80");
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");

        this.dynamoDbContainer = new DynamoDbContainer();
        this.factory = new WebApplicationFactory<Program>();        
    }

    internal HttpClient GetClient()
    {
        var client = factory.CreateClient();
        client.BaseAddress = factory.Server.BaseAddress;
        return client;
    }

    public async Task DisposeAsync()
    {
        this.factory.Dispose();
        await this.dynamoDbContainer.DisposeAsync();
    }

    public async Task InitializeAsync()
    {
        await dynamoDbContainer.StartAsync();
        Environment.SetEnvironmentVariable("DATABASE_DYNAMO", dynamoDbContainer.GetConnectionString());
    }
}