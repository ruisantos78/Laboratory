using RuiSantos.ZocDoc.Core.Models;

namespace RuiSantos.ZocDoc.Core.Repositories;

public interface IAppointamentsRepository
{
    Task RemoveAsync(Doctor doctor, Patient patient, Appointment appointment);
    Task StoreAsync(Doctor doctor, Patient patient, Appointment appointment);
}
