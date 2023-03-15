using Microsoft.Extensions.Logging;
using RuiSantos.ZocDoc.Core.Adapters;
using RuiSantos.ZocDoc.Core.Data;
using RuiSantos.ZocDoc.Core.Managers.Exceptions;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Core.Resources;
using static RuiSantos.ZocDoc.Core.Resources.ManagementUtils;

namespace RuiSantos.ZocDoc.Core.Managers;

/// <summary>
/// Manages the medical specialties.
/// </summary>
internal class MedicalSpecialtiesManagement : IMedicalSpecialtiesManagement
{
    /// <summary>
    /// The data context.
    /// </summary>
    private readonly IMedicalSpecialityAdapter medicalSpecialityAdapter;
    private readonly IDoctorAdapter doctorAdapter;

    /// <summary>
    /// The logger.
    /// </summary>
    private readonly ILogger logger;

    /// <summary>
    /// Creates a new instance of the medical specialties management.
    /// </summary>
    /// <param name="medicalSpecialityAdapter">The medical specialities adapter.</param>
    /// <param name="logger">The logger.</param>
    public MedicalSpecialtiesManagement(IMedicalSpecialityAdapter medicalSpecialityAdapter,
                                        IDoctorAdapter doctorAdapter,
                                        ILogger<MedicalSpecialtiesManagement> logger)
    {
        this.medicalSpecialityAdapter = medicalSpecialityAdapter;
        this.doctorAdapter = doctorAdapter;
        this.logger = logger;
    }

    /// <summary>
    /// Creates one or more medical specialties.
    /// </summary>
    /// <param name="descriptions">The medical specialties descriptions.</param>
    /// <exception cref="ValidationFailException">Thrown when the validation fails.</exception>
    /// <exception cref="ManagementFailException">Thrown when the management fails.</exception>
    public async Task CreateMedicalSpecialtiesAsync(List<string> decriptions)
    {
        try
        {
            foreach (var description in decriptions)
            {
                if (await medicalSpecialityAdapter.ContainsAsync(description))
                    continue;

                var model = new MedicalSpeciality(description);
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

    /// <summary>
    /// Removes one medical specialties.
    /// </summary>
    /// <param name="description">The medical specialties description.</param>
    /// <exception cref="ValidationFailException">Thrown when the validation fails.</exception>
    /// <exception cref="ManagementFailException">Thrown when the management fails.</exception>
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
                doctor.Specialities.Remove(description);
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

    /// <summary>
    /// Gets all the medical specialties.
    /// </summary>
    /// <returns>The medical specialties.</returns>
    /// <exception cref="ManagementFailException">Thrown when the management fails.</exception>
    public async Task<List<MedicalSpeciality>> GetMedicalSpecialitiesAsync()
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