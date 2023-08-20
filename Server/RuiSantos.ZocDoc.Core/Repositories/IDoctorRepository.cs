using RuiSantos.ZocDoc.Core.Models;

namespace RuiSantos.ZocDoc.Core.Repositories;

public interface IDoctorRepository
{
    Task<Doctor?> FindAsync(string license);    
    Task<List<Doctor>> FindBySpecialityAsync(string specialty);
    Task<List<Doctor>> FindBySpecialtyWithAvailabilityAsync(string specialty, DateOnly date);
    Task<List<Doctor>> FindAllWithAppointmentsAsync(IEnumerable<Appointment> appointments);

    Task StoreAsync(Doctor doctor);
}
