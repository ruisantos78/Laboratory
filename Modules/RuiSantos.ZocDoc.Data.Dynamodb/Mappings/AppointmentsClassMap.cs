using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Data.Dynamodb.Mappings.Core;
using RuiSantos.ZocDoc.Data.Dynamodb.Mediators;

using static RuiSantos.ZocDoc.Data.Dynamodb.Mappings.ClassMapConstants;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Mappings;

internal class AppointmentsClassMap : IRegisterClassMap
{
    public CreateTableRequest GetCreateTableRequest() => new()
    {
        TableName = AppointmentsTableName,
        AttributeDefinitions = new List<AttributeDefinition> {            
            new(AppointmentIdAttributeName, ScalarAttributeType.S),
            new(DoctorIdAttributeName, ScalarAttributeType.S),
            new(PatientIdAttributeName, ScalarAttributeType.S),
            new(AppointmentDateTimeAttributeName, ScalarAttributeType.S)
        },
        KeySchema = new List<KeySchemaElement>() {
            new(AppointmentIdAttributeName, KeyType.HASH)
        },
        GlobalSecondaryIndexes = new List<GlobalSecondaryIndex>
        {
            new GlobalSecondaryIndexHashKey(DoctorAppointmentsIndexName, DoctorIdAttributeName, AppointmentDateTimeAttributeName),
            new GlobalSecondaryIndexHashKey(PatientAppointmentsIndexName, PatientIdAttributeName, AppointmentDateTimeAttributeName)
        },
        ProvisionedThroughput = new(5, 5)
    };
}

