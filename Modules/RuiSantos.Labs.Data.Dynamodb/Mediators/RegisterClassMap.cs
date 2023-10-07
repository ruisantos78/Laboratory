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
}