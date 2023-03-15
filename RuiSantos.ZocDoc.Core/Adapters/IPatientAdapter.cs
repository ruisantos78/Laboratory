using RuiSantos.ZocDoc.Core.Models;

namespace RuiSantos.ZocDoc.Core.Adapters;

public interface IPatientAdapter 
{
    Task<Patient?> FindAsync(string socialSecurityNumber);
    Task<List<Patient>> FindAllWithAppointmentsAsync(List<Appointment> appointments);
    
    Task StoreAsync(Patient patient);
}