using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using RuiSantos.Labs.Data.Dynamodb.Mappings.Core;
using RuiSantos.Labs.Data.Dynamodb.Mediators;

using static RuiSantos.Labs.Data.Dynamodb.Mappings.ClassMapConstants;

namespace RuiSantos.Labs.Data.Dynamodb.Mappings;

internal class DoctorSpecialtyClassMap: IRegisterClassMap
{
    public CreateTableRequest GetCreateTableRequest() => new()
    {       
        TableName = DoctorSpecialtiesTableName,
        AttributeDefinitions = new List<AttributeDefinition> {
            new(DoctorIdAttributeName, ScalarAttributeType.S),
            new(SpecialtyAttributeName, ScalarAttributeType.S),
        },
        KeySchema = new List<KeySchemaElement> {
            new(DoctorIdAttributeName, KeyType.HASH),
            new(SpecialtyAttributeName, KeyType.RANGE)
        },
        GlobalSecondaryIndexes = new List<GlobalSecondaryIndex>
        {
            new GlobalSecondaryIndexHashKey(DoctorSpecialtyIndexName, SpecialtyAttributeName, DoctorIdAttributeName)            
        },
        ProvisionedThroughput = new(5, 5)
    };
}