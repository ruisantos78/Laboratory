using RuiSantos.Labs.Core.Repositories;
using RuiSantos.Labs.Data.Dynamodb.Adapters;
using RuiSantos.Labs.Domain.Models;

namespace RuiSantos.Labs.Data.Dynamodb.Repositories;

public class AppointmentsRepository(
    IPatientAdapter patientAdapter,
    IAppointmentAdapter appointmentAdapter)
    : IAppointmentsRepository
{
    public Task<Appointment?> GetAsync(Patient patient, DateTime dateTime)
    {
        return appointmentAdapter.FindByPatientAsync(patient, dateTime);
    }

    public Task<Appointment?> GetAsync(Doctor doctor, DateTime dateTime)
    {
        return appointmentAdapter.FindByDoctorAsync(doctor, dateTime);
    }

    public async Task<IEnumerable<PatientAppointment>> GetPatientAppointmentsAsync(Doctor doctor, DateOnly date,
        CancellationToken cancellationToken)
    {
        var tasks = await appointmentAdapter.LoadByDoctorAsync(doctor, date, cancellationToken)
            .ContinueWith(task => task.Result
                .Select(patientAdapter.GetAppointmentAsync)
                .OfType<Task<PatientAppointment>>(), cancellationToken);

        return await Task.WhenAll(tasks);
    }

    public Task RemoveAsync(Appointment appointment)
    {
        return appointmentAdapter.RemoveAsync(appointment);
    }

    public Task StoreAsync(Doctor doctor, Patient patient, DateTime dateTime)
    {
        return appointmentAdapter.StoreAsync(doctor, patient, dateTime);
    }
}