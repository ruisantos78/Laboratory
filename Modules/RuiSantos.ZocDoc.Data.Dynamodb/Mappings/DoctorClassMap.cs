using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Mappings;

internal class DoctorClassMap : IRegisterClassMap
{
    public CreateTableRequest GetCreateTableRequest() => new()
    {
        TableName = "Doctors",
        AttributeDefinitions = new List<AttributeDefinition>
        {
            new("Id", ScalarAttributeType.S),
            new("License", ScalarAttributeType.S)
        },
        KeySchema = new List<KeySchemaElement>
        {
            new("Id", KeyType.HASH)
        },
        GlobalSecondaryIndexes = new List<GlobalSecondaryIndex>
        {
            new GlobalSecondaryIndexHashKey("DoctorLicenseIndex", "License")            
        },
        ProvisionedThroughput = new(5, 5)
    };
}