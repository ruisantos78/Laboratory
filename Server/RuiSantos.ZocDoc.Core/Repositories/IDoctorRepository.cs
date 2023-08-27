using RuiSantos.ZocDoc.Core.Models;

namespace RuiSantos.ZocDoc.Core.Repositories;

public interface IDoctorRepository
{
    Task<Doctor?> FindAsync(string license);

    Task<List<Doctor>> FindBySpecialityAsync(string specialty);
    
    Task<List<Doctor>> FindByAppointmentsAsync(IEnumerable<Appointment> appointments);

    IAsyncEnumerable<DoctorSchedule> FindBySpecialtyWithAvailabilityAsync(string specialty, DateOnly date);

    Task StoreAsync(Doctor doctor);
}
