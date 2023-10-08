using System.Net;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RuiSantos.Labs.Data.Dynamodb.Mediators;

namespace RuiSantos.Labs.Infrastrucutre.Tests.Containers;

// ReSharper disable LocalizableElement
public sealed class LocalstackContainer : IAsyncDisposable
{
    private const int DefaultPort = 4566;

    private static DateTime Now => DateTime.Now;

    private const string RepositoySourceFile = @"Assets/Repository.json";

    private readonly IContainer _container;

    public LocalstackContainer()
    {
        _container = new ContainerBuilder()
            .WithImage("localstack/localstack")
            .WithAutoRemove(true)
            .WithPortBinding(DefaultPort, true)
            .WithEnvironment(new Dictionary<string, string>
            {
                { "DOCKER_HOST", "unix:///var/run/docker.sock" },
                { "DYNAMODB_SHARE_DB", "1" },
                { "SERVICES", "dynamodb:4569" },
                { "DEFAULT_REGION", "us-east-1" }
            })
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request =>
                request.ForPath("/")
                    .ForPort(DefaultPort)
                    .ForStatusCode(HttpStatusCode.OK)
            ))
            .Build();
    }

    public string GetEndpoint() => $"http://{_container.Hostname}:{_container.GetMappedPublicPort(DefaultPort)}";

    public IAmazonDynamoDB GetDynamoDbClient() => new AmazonDynamoDBClient("docker", "docker", new AmazonDynamoDBConfig
    {
        Profile = new Profile("default", "us-east-1"),
        ServiceURL = GetEndpoint()
    });

    public async ValueTask DisposeAsync()
    {
        await _container.StopAsync();
        await _container.DisposeAsync();
    }

    public async Task StartAsync()
    {
        await _container.StartAsync();
        Console.WriteLine(
            $"[ruisantos {Now:HH:mm:ss}] Start localstack at port: {_container.GetMappedPublicPort(DefaultPort)}");

        await InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        using var client = GetDynamoDbClient();

        Console.WriteLine($"[ruisantos {Now:HH:mm:ss}] Start Create Dynamodb Tables");
        var context = new DynamoDBContext(client);
        var repository = await JToken.ReadFromAsync(new JsonTextReader(new StreamReader(RepositoySourceFile)));

        var mappings = RegisterClassMaps.TableEntities();
        var requests = RegisterClassMaps.CreateTableRequests()
            .Select(x => client.CreateTableAsync(x))
            .ToArray();

        var tables = await Task.WhenAll(requests);
        foreach (var table in tables)
        {
            var tableName = table.TableDescription.TableName;
            if (repository[tableName] is not { } token || !mappings.TryGetValue(tableName, out var entityType))
            {
                Console.WriteLine(
                    $"[ruisantos {Now:HH:mm:ss}] # {tableName} - {table.TableDescription.TableStatus} - 0 records.");
                continue;
            }

            var entities = token.Select(t => t.ToObject(entityType)).ToArray();

            var writer = context.CreateBatchWrite(entityType);
            writer.AddPutItems(entities);
            await writer.ExecuteAsync();

            Console.WriteLine($"[ruisantos {Now:HH:mm:ss}] # {tableName} - {table.TableDescription.TableStatus} - {entities.Length} records.");
        }
    }
}
// ReSharper enable LocalizableElement