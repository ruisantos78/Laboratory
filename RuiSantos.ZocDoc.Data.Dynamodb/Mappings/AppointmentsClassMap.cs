using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Mappings;

public class AppointmentsClassMap: IRegisterClassMap
{
    public CreateTableRequest GetCreateTableRequest() => new()
    {
        TableName = "Appointments",
        AttributeDefinitions = new List<AttributeDefinition> {
            new("AppointmentId", ScalarAttributeType.S),
            new("DoctorId", ScalarAttributeType.S),
            new("PatientId", ScalarAttributeType.S)
        },
        KeySchema = new List<KeySchemaElement>() {
            new("AppointmentId", KeyType.HASH)
        },
        GlobalSecondaryIndexes = new List<GlobalSecondaryIndex> {
            new GlobalSecondaryIndexHashKey("DoctorAppointmentIndex", "DoctorId"),
            new GlobalSecondaryIndexHashKey("PatientAppointmentIndex", "PatientId")
        },
        ProvisionedThroughput = new(5, 5)
    };    
}

