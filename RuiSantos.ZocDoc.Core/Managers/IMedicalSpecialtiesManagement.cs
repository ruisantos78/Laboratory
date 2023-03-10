using RuiSantos.ZocDoc.Core.Models;

namespace RuiSantos.ZocDoc.Core.Managers;

/// <summary>
/// Interface for managing medical specialties.
/// </summary>
public interface IMedicalSpecialtiesManagement
{
    /// <summary>
    /// Creates a new medical specialty.
    /// </summary>
    /// <param name="description">The description of the specialty.</param>
    Task CreateMedicalSpecialtiesAsync(IEnumerable<string> decriptions);

    /// <summary>
    /// Gets all the medical specialties.
    /// </summary>
    /// <returns>A list of medical specialties.</returns>
    Task<List<MedicalSpeciality>> GetMedicalSpecialitiesAsync();

    /// <summary>
    /// Removes a medical specialty.
    /// </summary>
    /// <param name="description">The description of the specialty.</param>
    Task RemoveMedicalSpecialtiesAsync(string description);
}
