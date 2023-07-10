using Amazon.DynamoDBv2.DataModel;
using RuiSantos.ZocDoc.Data.Dynamodb.Entities.Converters;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Entities;

[DynamoDBTable("Appointments")]
internal class AppointmentsDto
{
    const string DoctorAppointmentIndex = "DoctorAppointmentIndex";
    const string PatientAppointmentIndex = "PatientAppointmentIndex";

    [DynamoDBHashKey(typeof(GuidConverter))]
    public Guid AppointmentId { get; init; } = Guid.NewGuid();
    
    [DynamoDBGlobalSecondaryIndexHashKey(DoctorAppointmentIndex, Converter = typeof(GuidConverter))]
    public Guid DoctorId { get; set; }

    [DynamoDBGlobalSecondaryIndexHashKey(PatientAppointmentIndex, Converter = typeof(GuidConverter))]
    public Guid PatientId { get; set; }

    [DynamoDBProperty(typeof(DateTimeConverter))]
    public DateTime AppointmentTime { get; set; }

    public static async Task<IReadOnlyList<(DateTime Date, Guid Id)>> GetAppointmentsByDoctorIdAsync(IDynamoDBContext context, Guid doctorId)
    {
        var appointments = await context.QueryAsync<AppointmentsDto>(doctorId, new DynamoDBOperationConfig
        {
            IndexName = DoctorAppointmentIndex
        }).GetRemainingAsync();
        
        return appointments.Select(a => (a.AppointmentTime, a.AppointmentId)).ToList();
    }

    public static async Task<IReadOnlyList<(DateTime Date, Guid Id)>> GetAppointmentsByPatientIdAsync(IDynamoDBContext context, Guid patientId)
    {
        var appointments = await context.QueryAsync<AppointmentsDto>(patientId, new DynamoDBOperationConfig
        {
            IndexName = PatientAppointmentIndex
        }).GetRemainingAsync();

        return appointments.Select(a => (a.AppointmentTime, a.AppointmentId)).ToList();
    }

    public static async Task<IReadOnlyList<Guid>> GetDoctorIdsByAppointmentAsync(IDynamoDBContext context, Guid appointmentId)
    {
        var appointments = await context.QueryAsync<AppointmentsDto>(appointmentId).GetRemainingAsync();
        return appointments.Select(a => a.DoctorId).ToList();
    }
}