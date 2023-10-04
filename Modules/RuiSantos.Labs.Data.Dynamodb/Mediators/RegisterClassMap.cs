using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;

namespace RuiSantos.Labs.Data.Dynamodb.Mediators;

internal interface IRegisterClassMap
{
    CreateTableRequest CreateTableRequest();
}

internal static class RegisterClassMaps {
    public static IEnumerable<CreateTableRequest> CreateTableRequests() => typeof(IRegisterClassMap).Assembly
        .GetTypes()
        .Where(t => t.GetInterfaces().Contains(typeof(IRegisterClassMap)))
        .Select(Activator.CreateInstance)
        .OfType<IRegisterClassMap>()
        .Select(i => i.CreateTableRequest());

    public static IReadOnlyDictionary<string, Type> TableEntities() => typeof(IRegisterClassMap).Assembly
        .GetTypes()
        .Where(t => t.GetCustomAttributes(false).OfType<DynamoDBTableAttribute>().Any())
        .ToDictionary(
            k => k.GetCustomAttributes(false).OfType<DynamoDBTableAttribute>().First().TableName,
            v => v            
        );
    
    public static async Task InitializeDatabaseAsync(IAmazonDynamoDB client)
    {
        var server = await client.ListTablesAsync().ConfigureAwait(false);

        var tasks = CreateTableRequests()
            .Where(req => !server.TableNames.Contains(req.TableName))
            .Select(req => client.CreateTableAsync(req));

        await Task.WhenAll(tasks);
    }
}