using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using RuiSantos.ZocDoc.Data.Dynamodb.Mediators;
using RuiSantos.ZocDoc.Data.Dynamodb.Mappings.Core;

using static RuiSantos.ZocDoc.Data.Dynamodb.Mappings.ClassMapConstants;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Mappings;

internal class PatientClassMap : IRegisterClassMap
{
    public CreateTableRequest GetCreateTableRequest() => new()
    {
        TableName = PatientsTableName,
        AttributeDefinitions = new List<AttributeDefinition>
        {
            new(IdAttributeName, ScalarAttributeType.S),
            new(SocialSecurityNumberAttributeName, ScalarAttributeType.S)
        },
        KeySchema = new List<KeySchemaElement>
        {
            new(IdAttributeName, KeyType.HASH)
        },
        GlobalSecondaryIndexes = new List<GlobalSecondaryIndex>
        {
            new GlobalSecondaryIndexHashKey(PatientSocialSecurityNumberIndexName, SocialSecurityNumberAttributeName)            
        },
        ProvisionedThroughput = new(5, 5)
    };
}
