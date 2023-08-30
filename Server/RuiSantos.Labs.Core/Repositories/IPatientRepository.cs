using RuiSantos.Labs.Core.Models;

namespace RuiSantos.Labs.Core.Repositories;

public interface IPatientRepository 
{
    Task<Patient?> FindAsync(string socialSecurityNumber);

    Task StoreAsync(Patient patient);
}