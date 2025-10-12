using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Microsoft.AspNetCore.Mvc.Testing;
using RuiSantos.Labs.Data.Dynamodb.Mediators;
using Testcontainers.LocalStack;
using Xunit.Abstractions;

namespace RuiSantos.Labs.Services.Tests.Fixtures;

[CollectionDefinition(nameof(ServiceFixture))]
public class ServiceCollectionFixture : ICollectionFixture<ServiceFixture>;

public class ServiceFixture: IAsyncLifetime
{
    private static Lazy<CreateTableRequest[]> CreateTableRequests => new(() => [..RegisterClassMaps.CreateTableRequests()]); 
    
    private WebApplicationFactory<Program>? _webApplicationFactory;
    private LocalStackContainer? _localStackContainer;
    private IAmazonDynamoDB? _amazonDynamoDb;

    public async Task InitializeAsync()
    {
        await InitializeLocalStackContainer();
        InitializeWebApplicationFactory();
    }

    public async Task DisposeAsync()
    {
        if (_localStackContainer is not null)
            await _localStackContainer.DisposeAsync();
        
        if (_webApplicationFactory is not null)
            await _webApplicationFactory.DisposeAsync();
    }
    
    private void InitializeWebApplicationFactory()
    {
        Environment.SetEnvironmentVariable("AWS_ACCESS_KEY_ID", "docker");
        Environment.SetEnvironmentVariable("AWS_SECRET_ACCESS_KEY", "docker");
        Environment.SetEnvironmentVariable("AWS_DEFAULT_REGION", "us-east-1");
        Environment.SetEnvironmentVariable("AWS_ENDPOINT_URL", _localStackContainer?.GetConnectionString());
        
        _webApplicationFactory = new WebApplicationFactory<Program>();
    }
    
    private async Task InitializeLocalStackContainer()
    {
        _localStackContainer = new LocalStackBuilder()
            .WithImage("localstack/localstack:latest")
            .WithEnvironment("DEFAULT_REGION", "us-east-1")
            .WithEnvironment("SERVICES", "dynamodb")
            .WithEnvironment("DYNAMODB_SHARE_DB", "1")
            .Build();
        
        await _localStackContainer.StartAsync();
        
        _amazonDynamoDb = new AmazonDynamoDBClient("docker", "docker", new AmazonDynamoDBConfig
        {
            Profile = new Profile("default", "us-east-1"),
            ServiceURL = _localStackContainer.GetConnectionString()
        });
        
        await Task.WhenAll(CreateTableRequests.Value.Select(request => _amazonDynamoDb.CreateTableAsync(request)));
    }
    
    public HttpClient CreateHttpClient()
    {
        ArgumentNullException.ThrowIfNull(_webApplicationFactory);
        return _webApplicationFactory.CreateClient();
    }

    public DynamoDBContext CreateDynamoDbContext()
    {
        ArgumentNullException.ThrowIfNull(_amazonDynamoDb);
        return new DynamoDBContextBuilder().WithDynamoDBClient(() => _amazonDynamoDb).Build();
    }
}