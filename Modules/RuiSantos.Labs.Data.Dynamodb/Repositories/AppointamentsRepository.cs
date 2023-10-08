using RuiSantos.Labs.Core.Models;
using RuiSantos.Labs.Core.Repositories;
using RuiSantos.Labs.Data.Dynamodb.Adapters;

namespace RuiSantos.Labs.Data.Dynamodb.Repositories;

public class AppointamentsRepository : IAppointamentsRepository
{
    private readonly IAppointmentAdapter _appointmentAdapter;
    private readonly IPatientAdapter _patientAdapter;

    public AppointamentsRepository(
        IPatientAdapter patientAdapter,
        IAppointmentAdapter appointmentAdapter)
    {
        _patientAdapter = patientAdapter;
        _appointmentAdapter = appointmentAdapter;
    }

    public Task<Appointment?> GetAsync(Patient patient, DateTime dateTime)
    {
        return _appointmentAdapter.FindByPatientAsync(patient, dateTime);
    }

    public Task<Appointment?> GetAsync(Doctor doctor, DateTime dateTime)
    {
        return _appointmentAdapter.FindByDoctorAsync(doctor, dateTime);
    }

    public async Task<IEnumerable<PatientAppointment>> GetPatientAppointmentsAsync(Doctor doctor, DateOnly date)
    {
        var tasks = await _appointmentAdapter.LoadByDoctorAsync(doctor, date)
            .ContinueWith(task => task.Result
                .Select(_patientAdapter.GetAppointmentAsync)
                .OfType<Task<PatientAppointment>>()
            );

        return await Task.WhenAll(tasks);
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