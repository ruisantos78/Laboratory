using RuiSantos.Labs.Core.Models;

namespace RuiSantos.Labs.Core.Repositories;

public interface IPatientRepository 
{
    /// <summary>
    /// Finds a patient by social security number. This is a synchronous method. It will return a Patient object when the request completes.
    /// </summary>
    /// <param name="socialSecurityNumber">The social security number to search for.</param>
    /// <returns>The patient that was found or null if none was found in the database ( for example if the social security number is not valid</returns>
    Task<Patient?> FindAsync(string socialSecurityNumber);

    /// <summary>
    /// Stores a patient in the data store. This is the asynchronous version of the store method. It does not wait for the result.
    /// </summary>
    /// <param name="patient">The patient to store. Must not be null.</param>
    /// <returns>A task representing the asynchronous operation. When the task completes the property will contain the patient that was stored</returns>
    Task StoreAsync(Patient patient);
}