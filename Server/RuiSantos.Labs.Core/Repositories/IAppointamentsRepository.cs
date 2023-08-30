using RuiSantos.Labs.Core.Models;

namespace RuiSantos.Labs.Core.Repositories;

public interface IAppointamentsRepository
{
    Task<Appointment?> GetAsync(Patient patient, DateTime dateTime);
    Task<Appointment?> GetAsync(Doctor doctor, DateTime dateTime);

    Task<IEnumerable<PatientAppointment>> GetPatientAppointmentsAsync(Doctor doctor, DateOnly date);

    Task RemoveAsync(Appointment appointment);

    Task StoreAsync(Doctor doctor, Patient patient, DateTime dateTime);
}
