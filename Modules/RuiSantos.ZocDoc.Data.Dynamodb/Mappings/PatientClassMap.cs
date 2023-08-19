using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using RuiSantos.ZocDoc.Data.Dynamodb.Mappings;

namespace RuiSantos.ZocDoc.Data.Dynamodb;

internal class PatientClassMap : IRegisterClassMap
{
    public CreateTableRequest GetCreateTableRequest() => new()
    {
        TableName = "Patients",
        AttributeDefinitions = new List<AttributeDefinition>
        {
            new("Id", ScalarAttributeType.S),
            new("SocialSecurityNumber", ScalarAttributeType.S)
        },
        KeySchema = new List<KeySchemaElement>
        {
            new("Id", KeyType.HASH)
        },
        GlobalSecondaryIndexes = new List<GlobalSecondaryIndex>
        {
            new GlobalSecondaryIndexHashKey("PatientSocialSecurityNumberIndex", "SocialSecurityNumber")            
        },
        ProvisionedThroughput = new(5, 5)
    };
}
