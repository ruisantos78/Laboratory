using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Core.Repositories;
using RuiSantos.ZocDoc.Data.Dynamodb.Entities;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Repositories;

public class AppointamentsRepository : IAppointamentsRepository
{
    private readonly DynamoDBContext context;

    public AppointamentsRepository(IAmazonDynamoDB client)
    {
        this.context = new DynamoDBContext(client);
    }

    public async Task<Appointment?> GetAsync(Patient patient, DateTime dateTime)
    {
        return await AppointmentsDto.GetAppointmentByPatientAsync(context, patient, dateTime);
    }

    public async Task<Appointment?> GetAsync(Doctor doctor, DateTime dateTime)
    {
        return await AppointmentsDto.GetAppointmentByDoctorAsync(context, doctor, dateTime);
    }

    public async Task<IEnumerable<PatientAppointment>> GetPatientAppointmentsAsync(Doctor doctor, DateOnly date)
    {
        var appointments = await AppointmentsDto.GetAppointmentsByDoctorAsync(context, doctor, date);

        return await PatientDto.GetPatientAppointmentsAsync(context, appointments);        
    }

    public async Task RemoveAsync(Appointment appointment)
    {
        await AppointmentsDto.RemoveAsync(context, appointment);
    }

    public async Task StoreAsync(Doctor doctor, Patient patient, DateTime dateTime)
    {
        await AppointmentsDto.StoreAsync(context, doctor, patient, dateTime);
    }
}