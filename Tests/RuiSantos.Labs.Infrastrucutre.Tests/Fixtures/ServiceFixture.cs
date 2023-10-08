using Amazon.DynamoDBv2.DataModel;
using Microsoft.AspNetCore.Mvc.Testing;
using RuiSantos.Labs.Infrastrucutre.Tests.Containers;

namespace RuiSantos.Labs.Infrastrucutre.Tests.Fixtures;

public sealed class ServiceFixture : IDisposable
{
    private readonly LocalstackContainer _localstack;
    private readonly WebApplicationFactory<Program> _application;

    public ServiceFixture()
    {
        _localstack = new LocalstackContainer();
        _localstack.StartAsync().Wait();

        Environment.SetEnvironmentVariable("ASPNETCORE_URLS", "http://+:55555");
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
        Environment.SetEnvironmentVariable("AWS_ACCESS_KEY_ID", "docker");
        Environment.SetEnvironmentVariable("AWS_SECRET_ACCESS_KEY", "docker");
        Environment.SetEnvironmentVariable("AWS_DEFAULT_REGION", "us-east-1");
        Environment.SetEnvironmentVariable("AWS_ENDPOINT_URL", _localstack.GetEndpoint());
        Environment.SetEnvironmentVariable("LABS_ALLOWED_ORIGINS", "http://localhost:8002");

        _application = new WebApplicationFactory<Program>();
    }

    internal IDynamoDBContext GetDynamoDbContext() => new DynamoDBContext(_localstack.GetDynamoDbClient());

    internal HttpClient GetHttpClient(string root = "/")
    {
        var client = _application.CreateClient();
        client.BaseAddress = new Uri(_application.Server.BaseAddress, root);
        return client;
    }

    public void Dispose()
    {
        Task.WaitAll(
            _application.DisposeAsync().AsTask(),
            _localstack.DisposeAsync().AsTask()
        );
    }
}