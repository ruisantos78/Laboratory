using System.Net;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RuiSantos.Labs.Data.Dynamodb.Mediators;
using static System.DateTime;

namespace RuiSantos.Labs.Api.Tests.Containers;

// ReSharper disable LocalizableElement
public sealed class DynamoDbContainer : IAsyncDisposable
{
    private const string RepositoySourceFile = @"Assets/Repository.json";

    private readonly IContainer _container;

    public DynamoDbContainer()
    {
        _container = new ContainerBuilder()
            .WithEntrypoint("java")
            .WithCommand("-jar", "DynamoDBLocal.jar", "-sharedDb")
            .WithImage("amazon/dynamodb-local:latest")
            .WithPortBinding(8000, true)
            .WithWaitStrategy(
                Wait.ForUnixContainer()
                    .UntilHttpRequestIsSucceeded(request =>
                        request.ForPath("/")
                               .ForPort(8000)
                               .ForStatusCode(HttpStatusCode.BadRequest)
                    )
            )
            .Build();
    }

    public string GetConnectionString() => $"http://{_container.Hostname}:{_container.GetMappedPublicPort(8000)}";
    public IAmazonDynamoDB GetClient() => new AmazonDynamoDBClient(new AmazonDynamoDBConfig
    {
        ServiceURL = GetConnectionString()
    });

    public async ValueTask DisposeAsync()
    {
        await _container.StopAsync();
        await _container.DisposeAsync();
    }

    public async Task StartAsync()
    {
        await _container.StartAsync();
        Console.WriteLine($"[ruisantos {Now:HH:mm:ss}] Start Dynamodb Client at port: {_container.GetMappedPublicPort(8000)}");

        await InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        using var client = GetClient();

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
            if (repository[tableName] is not {} token || !mappings.TryGetValue(tableName, out var entityType))
            {
                Console.WriteLine($"[ruisantos {Now:HH:mm:ss}] # {tableName} - {table.TableDescription.TableStatus} - 0 records.");
                continue;
            }

            var entities = token.Select(t => t.ToObject(entityType)).ToArray();

            var writer = context.CreateBatchWrite(entityType);
            writer.AddPutItems(entities);
            await writer.ExecuteAsync();

            Console.WriteLine($"[ruisantos {Now:HH:mm:ss}] # {tableName} - {table.TableDescription.TableStatus} - {entities.Count()} records.");
        }
    }
}
// ReSharper enable LocalizableElement
