﻿using System.Net;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RuiSantos.ZocDoc.Data.Dynamodb.Mappings;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Tests.Fixtures;

public sealed partial class DatabaseFixture : IAsyncLifetime
{
    private static Lazy<IReadOnlyDictionary<string, Type>> TypeMappings => new(() =>
        typeof(IRegisterClassMap).Assembly.GetTypes()
            .Where(t => t.GetCustomAttributes(false).OfType<DynamoDBTableAttribute>().Any())
            .ToDictionary(
                k => k.GetCustomAttributes(false).OfType<DynamoDBTableAttribute>().First().TableName,
                v => v
            )
    );

    private static Lazy<IReadOnlyList<CreateTableRequest>> TableRequests => new(() => 
        typeof(IRegisterClassMap).Assembly.GetTypes()
            .Where(t => t.GetInterfaces().Contains(typeof(IRegisterClassMap)))
            .Select(Activator.CreateInstance)
            .OfType<IRegisterClassMap>()
            .Select(map => map.GetCreateTableRequest())
            .ToArray()
    );

    public AmazonDynamoDBClient Client { get; private set; } = new AmazonDynamoDBClient();

    private readonly IContainer container;

    public DatabaseFixture()
    {
        // Create a Docker container for DynamoDB Local
        this.container = new ContainerBuilder()
            .WithImage("amazon/dynamodb-local:1.13.0")
            .WithPortBinding(8000, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request =>
                request.ForPath("/").ForPort(8000).ForStatusCode(HttpStatusCode.BadRequest)))
            .Build();
    }

    public async Task InitializeAsync()
    {
        await container.StartAsync();

        this.Client = new AmazonDynamoDBClient(new AmazonDynamoDBConfig
        {
            ServiceURL = $"http://{container.Hostname}:{container.GetMappedPublicPort(8000)}"
        });

        await InitializeDatabaseAsync(Client);
    }

    public async Task DisposeAsync()
    {
        this.Client.Dispose();

        await container.StopAsync().ConfigureAwait(false);
        await container.DisposeAsync().ConfigureAwait(false);
    }

    private static async Task InitializeDatabaseAsync(AmazonDynamoDBClient client)
    {
        Console.WriteLine($"[ruisantos.zocdoc {DateTime.Now:HH:mm:ss}] Start Create Dynamodb Tables");

        var context = new DynamoDBContext(client);
        var repository = await JToken.ReadFromAsync(new JsonTextReader(new StreamReader("Assets/repository.json")));

        var tasks = TableRequests.Value.Select(x => client.CreateTableAsync(x));
        var tables = await Task.WhenAll(tasks);
        foreach (var table in tables)
        {
            var tableName = table.TableDescription.TableName;
            if (repository[tableName] is not JToken token || !TypeMappings.Value.TryGetValue(tableName, out var entityType))
            {
                Console.WriteLine($"[ruisantos.zocdoc {DateTime.Now:HH:mm:ss}] # {tableName} - {table.TableDescription.TableStatus} - 0 records.");
                continue;
            }

            var entities = token.Select(t => t.ToObject(entityType)!);

            var writer = context.CreateBatchWrite(entityType);
            writer.AddPutItems(entities);
            await writer.ExecuteAsync();

            Console.WriteLine($"[ruisantos.zocdoc {DateTime.Now:HH:mm:ss}] # {tableName} - {table.TableDescription.TableStatus} - {entities.Count()} records.");
        }
    }
}