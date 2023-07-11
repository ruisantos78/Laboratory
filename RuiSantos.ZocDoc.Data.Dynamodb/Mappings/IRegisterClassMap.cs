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
        void OnListTablesResponse(Task<ListTablesResponse> task)
        {
            var tableNames = task.Result.TableNames;

            var tasks = GetCreateTableRequests()
                .Where(req => !tableNames.Contains(req.TableName))
                .Select(req => client.CreateTableAsync(req));

            Task.WaitAll(tasks.ToArray());
        };

        client.ListTablesAsync().ContinueWith(OnListTablesResponse);
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