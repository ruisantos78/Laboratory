using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Mappings;

public class DomainListsClassMap : IRegisterClassMap
{
    public CreateTableRequest GetCreateTableRequest() => new()
    {
        TableName = "DomainLists",
        AttributeDefinitions = new List<AttributeDefinition>
        {
            new("Source", ScalarAttributeType.S)
        },
        KeySchema = new List<KeySchemaElement>
        {
            new("Source", KeyType.HASH)
        },
        ProvisionedThroughput = new(5, 5)
    };
}

