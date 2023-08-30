using Amazon.DynamoDBv2.DataModel;
using Microsoft.AspNetCore.Mvc.Testing;
using RuiSantos.ZocDoc.API.Tests.Containers;

namespace RuiSantos.ZocDoc.API.Tests.Fixtures;

[CollectionDefinition(nameof(ServiceCollectionFixture))]
public class ServiceCollectionFixture: ICollectionFixture<ServiceFixture> { }

public class ServiceFixture : IAsyncLifetime
{
    private readonly DynamoDbContainer dynamoDbContainer;
    private readonly WebApplicationFactory<Program> factory;

    public ServiceFixture()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_URLS", "http://+:55555");
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");

        this.dynamoDbContainer = new DynamoDbContainer();
        this.factory = new WebApplicationFactory<Program>();
    }

    internal IDynamoDBContext GetContext() => new DynamoDBContext(dynamoDbContainer.GetClient());

    internal HttpClient GetClient(string root = "/")
    {
        var client = factory.CreateClient();
        client.BaseAddress = new Uri(factory.Server.BaseAddress, root);
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