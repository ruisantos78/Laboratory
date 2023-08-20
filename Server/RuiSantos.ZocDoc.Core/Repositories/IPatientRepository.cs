using RuiSantos.ZocDoc.Core.Models;

namespace RuiSantos.ZocDoc.Core.Repositories;

public interface IPatientRepository 
{
    Task<Patient?> FindAsync(string socialSecurityNumber);
    Task<List<Patient>> FindAllWithAppointmentsAsync(IEnumerable<Appointment> appointments);
    
    Task StoreAsync(Patient patient);
}