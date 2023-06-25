using Microsoft.Extensions.Logging;
using RuiSantos.ZocDoc.Core.Adapters;
using RuiSantos.ZocDoc.Core.Managers.Exceptions;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Core.Resources;
using static RuiSantos.ZocDoc.Core.Resources.ManagementUtils;

namespace RuiSantos.ZocDoc.Core.Managers;

/// <summary>
/// Interface for managing medical specialties.
/// </summary>
public interface IMedicalSpecialtiesManagement
{
    /// <summary>
    /// Creates a new medical specialty.
    /// </summary>
    /// <param name="decriptions">The description of the specialty.</param>
    /// <exception cref="ValidationFailException">Thrown when the validation fails.</exception>
    /// <exception cref="ManagementFailException">Thrown when the management fails.</exception>
    Task CreateMedicalSpecialtiesAsync(List<string> decriptions);

    /// <summary>
    /// Gets all the medical specialties.
    /// </summary>
    /// <returns>A list of medical specialties.</returns>
    /// <exception cref="ManagementFailException">Thrown when the management fails.</exception>
    Task<List<MedicalSpecialty>> GetMedicalSpecialitiesAsync();

    /// <summary>
    /// Removes a medical specialty.
    /// </summary>
    /// <param name="description">The description of the specialty.</param>
    /// <exception cref="ValidationFailException">Thrown when the validation fails.</exception>
    /// <exception cref="ManagementFailException">Thrown when the management fails.</exception>
    Task RemoveMedicalSpecialtiesAsync(string description);
}

internal class MedicalSpecialtiesManagement : IMedicalSpecialtiesManagement
{
    private readonly IMedicalSpecialityAdapter medicalSpecialityAdapter;
    private readonly IDoctorAdapter doctorAdapter;
    private readonly ILogger logger;

    public MedicalSpecialtiesManagement(IMedicalSpecialityAdapter medicalSpecialityAdapter,
                                        IDoctorAdapter doctorAdapter,
                                        ILogger<MedicalSpecialtiesManagement> logger)
    {
        this.medicalSpecialityAdapter = medicalSpecialityAdapter;
        this.doctorAdapter = doctorAdapter;
        this.logger = logger;
    }

    public async Task CreateMedicalSpecialtiesAsync(List<string> decriptions)
    {
        try
        {
            foreach (var description in decriptions)
            {
                if (await medicalSpecialityAdapter.ContainsAsync(description))
                    continue;

                var model = new MedicalSpecialty(description);
                if (!IsValid(model, out var validationException))
                    throw validationException;

                await medicalSpecialityAdapter.AddAsync(model);
            }
            
        }
        catch (ValidationFailException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger?.Fail(ex);
            throw new ManagementFailException(MessageResources.MedicalSpecialitiesSetFail);
        }
    }

    public async Task RemoveMedicalSpecialtiesAsync(string description)
    {
        try
        {
            if (await medicalSpecialityAdapter.ContainsAsync(description) == false) 
                throw new ValidationFailException(MessageResources.MedicalSpecialitiesDescriptionNotFound);

            await medicalSpecialityAdapter.RemoveAsync(description);

            var doctors = await doctorAdapter.FindBySpecialityAsync(description);
            foreach (var doctor in doctors)
            {
                doctor.Specialties.Remove(description);
                await doctorAdapter.StoreAsync(doctor);
            }            
        }
        catch (ValidationFailException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger?.Fail(ex);
            throw new ManagementFailException(MessageResources.MedicalSpecialitiesSetFail);
        }
    }

    public async Task<List<MedicalSpecialty>> GetMedicalSpecialitiesAsync()
    {
        try
        {
            return await medicalSpecialityAdapter.ToListAsync();
        }
        catch (Exception ex)
        {
            logger?.Fail(ex);
            throw new ManagementFailException(MessageResources.MedicalSpecialitiesGetFail);
        }
    }
}