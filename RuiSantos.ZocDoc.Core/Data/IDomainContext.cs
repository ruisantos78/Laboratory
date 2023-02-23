using RuiSantos.ZocDoc.Core.Models;

namespace RuiSantos.ZocDoc.Core.Data;

/// <summary>
/// Interface for accessing the domain context.
/// </summary>
public interface IDomainContext
{
    /// <summary>
    /// Gets all the medical specialties.
    /// </summary>
    /// <returns>A list of medical specialties.</returns>
    Task<List<MedicalSpeciality>?> GetMedicalSpecialtiesAsync();
}