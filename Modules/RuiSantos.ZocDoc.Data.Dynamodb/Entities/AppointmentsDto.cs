using Amazon.DynamoDBv2.DataModel;
using RuiSantos.ZocDoc.Data.Dynamodb.Entities.Converters;

using static RuiSantos.ZocDoc.Data.Dynamodb.Mappings.ClassMapConstants;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Entities;

[DynamoDBTable(AppointmentsTableName)]
internal partial class AppointmentsDto
{    
    [DynamoDBHashKey(
        AttributeName = AppointmentIdAttributeName, 
        Converter = typeof(GuidConverter))]
    public Guid AppointmentId { get; set; }

    [DynamoDBGlobalSecondaryIndexHashKey(DoctorAppointmentsIndexName, 
        AttributeName = DoctorIdAttributeName,
        Converter = typeof(GuidConverter))]
    public Guid DoctorId { get; set; }
    
    [DynamoDBGlobalSecondaryIndexHashKey(PatientAppointmentsIndexName, 
        AttributeName = PatientIdAttributeName,
        Converter = typeof(GuidConverter))]
    public Guid PatientId { get; set; }

    [DynamoDBGlobalSecondaryIndexRangeKey(DoctorAppointmentsIndexName, PatientAppointmentsIndexName,
        AttributeName = AppointmentDateTimeAttributeName,
        Converter = typeof(DateTimeConverter))]
    public DateTime AppointmentDateTime { get; set; }
}