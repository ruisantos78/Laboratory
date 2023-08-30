using RuiSantos.ZocDoc.Core.Models;

namespace RuiSantos.ZocDoc.Core.Repositories;

public interface IPatientRepository 
{
    Task<Patient?> FindAsync(string socialSecurityNumber);

    Task StoreAsync(Patient patient);
}