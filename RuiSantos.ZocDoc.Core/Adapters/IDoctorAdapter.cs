using RuiSantos.ZocDoc.Core.Models;

namespace RuiSantos.ZocDoc.Core.Adapters;

public interface IDoctorAdapter
{
    Task<Doctor?> FindAsync(string license);    
    Task<List<Doctor>> FindBySpecialityAsync(string specialty);
    Task<List<Doctor>> FindBySpecialtyWithAvailabilityAsync(string specialty, DateOnly date);
    Task<List<Doctor>> FindAllWithAppointmentsAsync(List<Appointment> appointments);

    Task StoreAsync(Doctor doctor);
}
