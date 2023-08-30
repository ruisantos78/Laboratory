using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using RuiSantos.Labs.Core.Models;
using RuiSantos.Labs.Data.Dynamodb.Mappings.Core;
using RuiSantos.Labs.Data.Dynamodb.Mediators;

using static RuiSantos.Labs.Data.Dynamodb.Mappings.ClassMapConstants;

namespace RuiSantos.Labs.Data.Dynamodb.Mappings;

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

