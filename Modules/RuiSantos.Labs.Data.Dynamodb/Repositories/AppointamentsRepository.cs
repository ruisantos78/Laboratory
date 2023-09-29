using Amazon.DynamoDBv2;
using RuiSantos.Labs.Core.Models;
using RuiSantos.Labs.Core.Repositories;
using RuiSantos.Labs.Data.Dynamodb.Adapters;

namespace RuiSantos.Labs.Data.Dynamodb.Repositories;

public class AppointamentsRepository : IAppointamentsRepository
{
    private readonly AppointmentAdapter appointmentAdapter;
    private readonly IAmazonDynamoDB client;

    public AppointamentsRepository(IAmazonDynamoDB client)
    {
        this.appointmentAdapter = new AppointmentAdapter(client);
        this.client = client;
    }

    public async Task<Appointment?> GetAsync(Patient patient, DateTime dateTime)
    {
        return await appointmentAdapter.FindByPatientAsync(patient, dateTime);
    }

    public async Task<Appointment?> GetAsync(Doctor doctor, DateTime dateTime)
    {
        return await appointmentAdapter.FindByDoctorAsync(doctor, dateTime);
    }

    public async IAsyncEnumerable<PatientAppointment> GetPatientAppointmentsAsync(Doctor doctor, DateOnly date)
    {
        var patientAdapter = new PatientAdapter(client);

        await foreach(var appointment in appointmentAdapter.LoadByDoctorAsync(doctor, date)) {
            if (await patientAdapter.GetAppointmentAsync(appointment) is {} patientAppointment)
                yield return patientAppointment;
        }
    }

    public async Task RemoveAsync(Appointment appointment)
    {
        await appointmentAdapter.RemoveAsync(appointment);
    }

    public async Task StoreAsync(Doctor doctor, Patient patient, DateTime dateTime)
    {
        await appointmentAdapter.StoreAsync(doctor, patient, dateTime);
    }
}