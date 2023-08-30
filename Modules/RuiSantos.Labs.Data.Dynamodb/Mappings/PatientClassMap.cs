using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using RuiSantos.Labs.Data.Dynamodb.Mediators;
using RuiSantos.Labs.Data.Dynamodb.Mappings.Core;

using static RuiSantos.Labs.Data.Dynamodb.Mappings.ClassMapConstants;

namespace RuiSantos.Labs.Data.Dynamodb.Mappings;

internal class PatientClassMap : IRegisterClassMap
{
    public CreateTableRequest GetCreateTableRequest() => new()
    {
        TableName = PatientsTableName,
        AttributeDefinitions = new List<AttributeDefinition>
        {
            new(PatientIdAttributeName, ScalarAttributeType.S),
            new(SocialSecurityNumberAttributeName, ScalarAttributeType.S)
        },
        KeySchema = new List<KeySchemaElement>
        {
            new(PatientIdAttributeName, KeyType.HASH)
        },
        GlobalSecondaryIndexes = new List<GlobalSecondaryIndex>
        {
            new GlobalSecondaryIndexHashKey(PatientSocialSecurityNumberIndexName, SocialSecurityNumberAttributeName)            
        },
        ProvisionedThroughput = new(5, 5)
    };
}
