using Amazon.DynamoDBv2.DataModel;
using Microsoft.AspNetCore.Mvc.Testing;
using RuiSantos.Labs.Infrastrucutre.Tests.Containers;

namespace RuiSantos.Labs.Infrastrucutre.Tests.Fixtures;

public sealed class ServiceFixture : IDisposable
{
    private readonly LocalstackContainer _dynamoDbContainer;
    private readonly WebApplicationFactory<Program> _factory;
    
    public ServiceFixture()
    {
        _dynamoDbContainer = new LocalstackContainer();
        _dynamoDbContainer.StartAsync().Wait();

        Environment.SetEnvironmentVariable("ASPNETCORE_URLS", "http://+:55555");
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
        Environment.SetEnvironmentVariable("AWS_ACCESS_KEY_ID", "docker");
        Environment.SetEnvironmentVariable("AWS_SECRET_ACCESS_KEY", "docker");
        Environment.SetEnvironmentVariable("AWS_DEFAULT_REGION", "us-east-1");
        Environment.SetEnvironmentVariable("AWS_ENDPOINT_URL", _dynamoDbContainer.GetConnectionString());
        Environment.SetEnvironmentVariable("LABS_ALLOWED_ORIGINS", "http://localhost:8002");

        _factory = new WebApplicationFactory<Program>();
    }

    internal IDynamoDBContext GetContext() => new DynamoDBContext(_dynamoDbContainer.GetClient());

    internal HttpClient GetClient(string root = "/")
    {
        var client = _factory.CreateClient();
        client.BaseAddress = new Uri(_factory.Server.BaseAddress, root);
        return client;
    }    

    public void Dispose()
    {
        Task.WaitAll(_factory.DisposeAsync().AsTask(), _dynamoDbContainer.DisposeAsync().AsTask());
    }
}