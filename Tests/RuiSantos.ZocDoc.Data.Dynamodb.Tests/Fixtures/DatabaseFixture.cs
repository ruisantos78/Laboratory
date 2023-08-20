using System.Net;
using Amazon.DynamoDBv2.DataModel;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RuiSantos.ZocDoc.Data.Dynamodb.Mediators;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Tests.Fixtures;

public sealed partial class DatabaseFixture : IAsyncLifetime
{
    public AmazonDynamoDBClient Client { get; private set; } = new AmazonDynamoDBClient();

    private readonly IContainer container;

    public DatabaseFixture()
    {
        // Create a Docker container for DynamoDB Local
        this.container = new ContainerBuilder()
            .WithPortBinding(8000, true)
            .WithEntrypoint("java")
            .WithCommand("-jar", "DynamoDBLocal.jar", "-sharedDb")
            .WithImage("amazon/dynamodb-local:latest")            
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request =>
                request.ForPath("/").ForPort(8000).ForStatusCode(HttpStatusCode.BadRequest)))
            .Build();        
    }

    public async Task InitializeAsync()
    {
        await container.StartAsync();

        var serviceUrl = $"http://{container.Hostname}:{container.GetMappedPublicPort(8000)}";
        this.Client = new AmazonDynamoDBClient(new AmazonDynamoDBConfig
        {
            ServiceURL = $"http://{container.Hostname}:{container.GetMappedPublicPort(8000)}"
        });

        Console.WriteLine($"[ruisantos.zocdoc {DateTime.Now:HH:mm:ss}] Start Dynamodb Client at {serviceUrl}");

        await InitializeDatabaseAsync();
    }

    public async Task DisposeAsync()
    {
        this.Client.Dispose();

        await container.StopAsync().ConfigureAwait(false);
        await container.DisposeAsync().ConfigureAwait(false);
    }

    private async Task InitializeDatabaseAsync()
    {
        Console.WriteLine($"[ruisantos.zocdoc {DateTime.Now:HH:mm:ss}] Start Create Dynamodb Tables");

        var context = new DynamoDBContext(Client);
        var repository = await JToken.ReadFromAsync(new JsonTextReader(new StreamReader("Assets/repository.json")));

        var mappings = RegisterClassMaps.GetTypeMappings();
        var requests = RegisterClassMaps.GetCreateTableRequests().Select(x => Client.CreateTableAsync(x));

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
