using System.Net;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RuiSantos.ZocDoc.Data.Dynamodb.Mediators;

namespace RuiSantos.ZocDoc.API.Tests.Containers;

public sealed partial class DynamoDbContainer : IAsyncDisposable
{
    private const string RepositoySourceFile = @"Assets/Repository.json";
    
    private IContainer? _container;

    public IContainer GetContainer()
    {
        return _container ??
            throw new InvalidOperationException("Docker container is not ready for the database client.");
    }

    public string GetConnectionString()
    {
        if (_container is null)
            throw new InvalidOperationException("Docker container is not ready for the database client.");

        return $"http:/{_container.Name}:8000";
    }

    public IAmazonDynamoDB GetClient()
    {
        if (_container is null)
            throw new InvalidOperationException("Docker container is not ready for the database client.");

        return new AmazonDynamoDBClient(new AmazonDynamoDBConfig
        {
            ServiceURL = $"http://{_container.Hostname}:{_container.GetMappedPublicPort(8000)}"
        });
    }

    public async ValueTask DisposeAsync()
    {
        if (_container is not null)
        {
            await _container.StopAsync();
            await _container.DisposeAsync();
        }
    }

    public async Task StartAsync(INetwork network)
    {
        _container = new ContainerBuilder()
            //.WithEntrypoint("java")
            //.WithCommand("-jar", "DynamoDBLocal.jar", "-sharedDb")
            .WithNetwork(network)
            .WithImage("amazon/dynamodb-local:latest")
            .WithPortBinding(8000, true)
            .WithEnvironment("AWS_ACCESS_KEY_ID", "api")
            .WithEnvironment("AWS_SECRET_ACCESS_KEY", "secret")
            .WithWaitStrategy(
                Wait.ForUnixContainer()
                    .UntilHttpRequestIsSucceeded(request =>
                        request.ForPath("/")
                               .ForPort(8000)
                               .ForStatusCode(HttpStatusCode.BadRequest)
                    )
            )
            .Build();

        await _container.StartAsync();
        Console.WriteLine($"[ruisantos.zocdoc {DateTime.Now:HH:mm:ss}] Start Dynamodb Client at port: {_container.GetMappedPublicPort(8000)}");

        await InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        using var client = GetClient();

        Console.WriteLine($"[ruisantos.zocdoc {DateTime.Now:HH:mm:ss}] Start Create Dynamodb Tables");
        var context = new DynamoDBContext(client);
        var repository = await JToken.ReadFromAsync(new JsonTextReader(new StreamReader(RepositoySourceFile)));

        var mappings = RegisterClassMaps.GetTypeMappings();
        var requests = RegisterClassMaps.GetCreateTableRequests()
            .Select(x => client.CreateTableAsync(x));

        var tables = await Task.WhenAll(requests);
        foreach (var table in tables)
        {
            var tableName = table.TableDescription.TableName;
            if (repository[tableName] is not JToken token || !mappings.TryGetValue(tableName, out var entityType))
            {
                Console.WriteLine($"[ruisantos.zocdoc {DateTime.Now:HH:mm:ss}] # {tableName} - {table.TableDescription.TableStatus} - 0 records.");
                continue;
            }

            var entities = token.Select(t => t.ToObject(entityType));

            var writer = context.CreateBatchWrite(entityType);
            writer.AddPutItems(entities);
            await writer.ExecuteAsync();

            Console.WriteLine($"[ruisantos.zocdoc {DateTime.Now:HH:mm:ss}] # {tableName} - {table.TableDescription.TableStatus} - {entities.Count()} records.");
        }
    }
}
