using Amazon.DynamoDBv2.DataModel;
using Microsoft.AspNetCore.Mvc.Testing;
using RuiSantos.Labs.Api.Tests.Containers;

namespace RuiSantos.Labs.Api.Tests.Fixtures;

[CollectionDefinition(nameof(ServiceCollectionFixture))]
public class ServiceCollectionFixture: ICollectionFixture<ServiceFixture> { }

public class ServiceFixture : IAsyncLifetime
{
    private readonly DynamoDbContainer _dynamoDbContainer;
    private readonly WebApplicationFactory<Program> _factory;
    
    public ServiceFixture()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_URLS", "http://+:55555");
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");

        _dynamoDbContainer = new DynamoDbContainer();
        _factory = new WebApplicationFactory<Program>();
    }

    internal IDynamoDBContext GetContext() => new DynamoDBContext(_dynamoDbContainer.GetClient());

    internal HttpClient GetClient(string root = "/")
    {
        var client = _factory.CreateClient();
        client.BaseAddress = new Uri(_factory.Server.BaseAddress, root);
        return client;
    }    

    public Task DisposeAsync() => Task.WhenAll(
        _factory.DisposeAsync().AsTask(),
        _dynamoDbContainer.DisposeAsync().AsTask()
    );

    public Task InitializeAsync() => _dynamoDbContainer.StartAsync()
        .ContinueWith(_ => Environment.SetEnvironmentVariable("DATABASE_DYNAMO", _dynamoDbContainer.GetConnectionString()));
}