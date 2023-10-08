using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using RuiSantos.Labs.Data.Dynamodb.Mediators;
using static RuiSantos.Labs.Data.Dynamodb.Mappings.MappingConstants;

namespace RuiSantos.Labs.Data.Dynamodb.Mappings;

internal class DictionaryClassMap : IRegisterClassMap
{
    public CreateTableRequest CreateTableRequest() => new()
    {
        TableName = DictionaryTableName,
        AttributeDefinitions = new List<AttributeDefinition>
        {
            new(SourceAttributeName, ScalarAttributeType.S),
            new(ValueAttributeName, ScalarAttributeType.S)
        },
        KeySchema = new List<KeySchemaElement>
        {
            new(SourceAttributeName, KeyType.HASH),
            new(ValueAttributeName, KeyType.RANGE)
        },
        ProvisionedThroughput = new(5, 5)
    };
}