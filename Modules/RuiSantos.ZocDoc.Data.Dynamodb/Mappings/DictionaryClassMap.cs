using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using  RuiSantos.ZocDoc.Data.Dynamodb.Mediators;

using static RuiSantos.ZocDoc.Data.Dynamodb.Mappings.ClassMapConstants;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Mappings;

internal class DictionaryClassMap : IRegisterClassMap
{
    public CreateTableRequest GetCreateTableRequest() => new()
    {
        TableName = DictionaryTableName,
        AttributeDefinitions = new List<AttributeDefinition>
        {
            new(SourceAttributeName, ScalarAttributeType.S)
        },
        KeySchema = new List<KeySchemaElement>
        {
            new(SourceAttributeName, KeyType.HASH)
        },
        ProvisionedThroughput = new(5, 5)
    };
}

