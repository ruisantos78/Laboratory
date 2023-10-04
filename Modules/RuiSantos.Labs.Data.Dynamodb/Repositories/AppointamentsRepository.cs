using Amazon.DynamoDBv2;
using RuiSantos.Labs.Core.Models;
using RuiSantos.Labs.Core.Repositories;
using RuiSantos.Labs.Data.Dynamodb.Adapters;

namespace RuiSantos.Labs.Data.Dynamodb.Repositories;

public class AppointamentsRepository : IAppointamentsRepository
{
    private readonly AppointmentAdapter _appointmentAdapter;
    private readonly Lazy<PatientAdapter> _patientAdapter;

    public AppointamentsRepository(IAmazonDynamoDB client)
    {
        _appointmentAdapter = new AppointmentAdapter(client);
        _patientAdapter = new Lazy<PatientAdapter>(new PatientAdapter(client));
    }

    public Task<Appointment?> GetAsync(Patient patient, DateTime dateTime)
    {
        return _appointmentAdapter.FindByPatientAsync(patient, dateTime);
    }

    public Task<Appointment?> GetAsync(Doctor doctor, DateTime dateTime)
    {
        return _appointmentAdapter.FindByDoctorAsync(doctor, dateTime);
    }

    public async IAsyncEnumerable<PatientAppointment> GetPatientAppointmentsAsync(Doctor doctor, DateOnly date)
    {
        var patientAdapter = _patientAdapter.Value;

        await foreach(var appointment in _appointmentAdapter.LoadByDoctorAsync(doctor, date)) {
            if (await patientAdapter.GetAppointmentAsync(appointment) is {} patientAppointment)
                yield return patientAppointment;
        }
    }

    public Task RemoveAsync(Appointment appointment)
    {
        return _appointmentAdapter.RemoveAsync(appointment);
    }

    public Task StoreAsync(Doctor doctor, Patient patient, DateTime dateTime)
    {
        return _appointmentAdapter.StoreAsync(doctor, patient, dateTime);
    }
}