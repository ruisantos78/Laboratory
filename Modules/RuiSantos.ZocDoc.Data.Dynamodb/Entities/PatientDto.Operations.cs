using Amazon.DynamoDBv2.DataModel;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Data.Dynamodb.Entities.Data;

using static RuiSantos.ZocDoc.Data.Dynamodb.Mappings.ClassMapConstants;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Entities;

partial class PatientDto
{
    public static async Task<Patient?> GetPatientBySocialSecurityNumberAsync(IDynamoDBContext context, string socialSecurityNumber)
    {
        var result = await DynamoOperations<Patient>.FindAllAsync<PatientDto>(context, PatientSocialSecurityNumberIndexName, socialSecurityNumber);
        return result.FirstOrDefault();
    }

    public static async Task<IEnumerable<PatientAppointment>> GetPatientAppointmentsAsync(IDynamoDBContext context, IEnumerable<Appointment> appointments)
    {
        var appointementsIds = appointments.Select(a => a.Id as object).ToList();
        var appointmentEntities = await DynamoOperations.FindListAsync<AppointmentsDto>(context, appointementsIds);

        var patientsIds = appointmentEntities.Select(a => a.PatientId as object).ToList();
        var patients = await DynamoOperations<Patient>.FindListAsync<PatientDto>(context, patientsIds);

        return appointmentEntities.Select(appointment => new PatientAppointment()
        {
            Patient = patients.First(p => p.Id == appointment.PatientId),
            Date = appointment.AppointmentDateTime
        });
    }

    public static async Task SetPatientAsync(IDynamoDBContext context, Patient patient) 
       => await DynamoOperations<Patient>.StoreAsync<PatientDto>(context, patient);  
}
