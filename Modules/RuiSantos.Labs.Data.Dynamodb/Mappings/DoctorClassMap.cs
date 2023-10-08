using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using RuiSantos.Labs.Data.Dynamodb.Mediators;
using RuiSantos.Labs.Data.Dynamodb.Mappings.Core;
using static RuiSantos.Labs.Data.Dynamodb.Mappings.MappingConstants;

namespace RuiSantos.Labs.Data.Dynamodb.Mappings;

internal class DoctorClassMap : IRegisterClassMap
{
    public CreateTableRequest CreateTableRequest() => new()
    {
        TableName = DoctorsTableName,
        AttributeDefinitions = new List<AttributeDefinition>
        {
            new(DoctorIdAttributeName, ScalarAttributeType.S),
            new(LicenseAttributeName, ScalarAttributeType.S)
        },
        KeySchema = new List<KeySchemaElement>
        {
            new(DoctorIdAttributeName, KeyType.HASH)
        },
        GlobalSecondaryIndexes = new List<GlobalSecondaryIndex>
        {
            new GlobalSecondaryIndexKeys(DoctorLicenseIndexName, LicenseAttributeName)
        },
        ProvisionedThroughput = new(5, 5)
    };
}