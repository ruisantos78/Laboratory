using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Mappings;

public class DoctorSpecialtyClassMap: IRegisterClassMap
{
    public CreateTableRequest GetCreateTableRequest() => new()
    {       
        TableName = "DoctorSpecialties",
        AttributeDefinitions = new List<AttributeDefinition> {
            new("DoctorId", ScalarAttributeType.S),
            new("Specialty", ScalarAttributeType.S),
        },
        KeySchema = new List<KeySchemaElement> {
            new("DoctorId", KeyType.HASH),
            new("Specialty", KeyType.RANGE)
        },
        GlobalSecondaryIndexes = new List<GlobalSecondaryIndex> {
            new GlobalSecondaryIndexHashKey("DoctorSpecialtyIndex", "Specialty")            
        },
        ProvisionedThroughput = new(5, 5)
    };
}