using Amazon.DynamoDBv2.DataModel;
using RuiSantos.ZocDoc.Data.Dynamodb.Entities.Converters;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Entities;

[DynamoDBTable("Appointments")]
internal class AppointmentsDto
{
    internal const string DoctorAppointmentIndex = "DoctorAppointmentIndex";
    internal const string PatientAppointmentIndex = "PatientAppointmentIndex";

    [DynamoDBHashKey(typeof(GuidConverter))]
    public Guid AppointmentId { get; init; } = Guid.NewGuid();
    
    [DynamoDBGlobalSecondaryIndexHashKey(DoctorAppointmentIndex, Converter = typeof(GuidConverter))]
    public Guid DoctorId { get; set; }

    [DynamoDBGlobalSecondaryIndexHashKey(PatientAppointmentIndex, Converter = typeof(GuidConverter))]
    public Guid PatientId { get; set; }

    [DynamoDBProperty(typeof(DateTimeConverter))]
    public DateTime AppointmentTime { get; set; }
}