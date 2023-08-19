using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Mappings;

internal interface IRegisterClassMap
{
    CreateTableRequest GetCreateTableRequest();
}

internal static class RegisterClassMaps {
    public static IEnumerable<CreateTableRequest> GetCreateTableRequests() => typeof(IRegisterClassMap).Assembly
        .GetTypes()
        .Where(t => t.GetInterfaces().Contains(typeof(IRegisterClassMap)))
        .Select(Activator.CreateInstance)
        .OfType<IRegisterClassMap>()
        .Select(i => i.GetCreateTableRequest());    

    public static void InitializeDatabase(AmazonDynamoDBClient client)
    {
        Task.WaitAll(Task.CompletedTask, InitializeDatabaseAsync(client));
    }

    public static async Task InitializeDatabaseAsync(AmazonDynamoDBClient client)
    {
        var server = await client.ListTablesAsync().ConfigureAwait(false);

        var tasks = GetCreateTableRequests()
            .Where(req => !server.TableNames.Contains(req.TableName))
            .Select(req => client.CreateTableAsync(req));

        await Task.WhenAll(tasks);
    }
}

public class GlobalSecondaryIndexHashKey: GlobalSecondaryIndex
{
    public GlobalSecondaryIndexHashKey(string indexName, string attributeName)
    {
        this.IndexName = indexName;

        this.KeySchema = new List<KeySchemaElement>
        {
            new(attributeName, KeyType.HASH)
        };

        this.Projection = new() { ProjectionType = ProjectionType.ALL };

        this.ProvisionedThroughput = new ProvisionedThroughput(5, 5);
    }
}