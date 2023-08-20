using Amazon.DynamoDBv2.DataModel;
using RuiSantos.ZocDoc.Data.Dynamodb.Entities.Converters;
using static RuiSantos.ZocDoc.Data.Dynamodb.Mappings.ClassMapConstants;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Entities;

[DynamoDBTable(AppointmentsTableName)]
internal class AppointmentsDto
{    
    [DynamoDBHashKey(
        AttributeName = AppointmentIdAttributeName, 
        Converter = typeof(GuidConverter))]
    public Guid AppointmentId { get; init; } = Guid.NewGuid();
    
    [DynamoDBGlobalSecondaryIndexHashKey(DoctorAppointmentIndexName, 
        AttributeName = DoctorIdAttributeName, 
        Converter = typeof(GuidConverter))]
    public Guid DoctorId { get; set; }

    [DynamoDBGlobalSecondaryIndexHashKey(PatientAppointmentIndexName, 
        AttributeName = PatientIdAttributeName, 
        Converter = typeof(GuidConverter))]
    public Guid PatientId { get; set; }

    [DynamoDBProperty(typeof(DateTimeConverter))]
    public DateTime AppointmentTime { get; set; }
}