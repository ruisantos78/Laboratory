using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace RuiSantos.Labs.Repository.DynamoDb.Core;

internal abstract class ClassMap
{
     private static ClassMap[] Mappers => typeof(ClassMap).Assembly
        .GetTypes()
        .Where(t => t.IsSubclassOf(typeof(ClassMap)))
        .Select(Activator.CreateInstance)
        .OfType<ClassMap>()
        .ToArray();
    
    protected abstract void Map(CreateTableRequest request);
    
    public static async Task CreateTablesAsync(IAmazonDynamoDB dynamoDb, CancellationToken cancellationToken = default)
    {
        var tables = await dynamoDb.ListTablesAsync(cancellationToken);
        
        foreach (var mapper in Mappers)
        {
            var request = new CreateTableRequest();
            mapper.Map(request);
            
            if (tables.TableNames.Contains(request.TableName))
                continue;

            await dynamoDb.CreateTableAsync(request, cancellationToken);
            await WaitForTableAsync(dynamoDb, request.TableName, cancellationToken);
        }
    }
    
    private static async Task WaitForTableAsync(IAmazonDynamoDB dynamoDb, string tableName, CancellationToken cancellationToken)
    {
        while (true)
        {
            try
            {
                var response = await dynamoDb.DescribeTableAsync(tableName, cancellationToken);
                if (response.Table.TableStatus == TableStatus.ACTIVE)
                    break;
            }
            catch (ResourceNotFoundException)
            {
                // Table not yet created
                await Task.Delay(100, cancellationToken);
            }
        }
    }
}