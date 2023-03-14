using Microsoft.Extensions.Logging;
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
    private readonly IDataContext context;

    /// <summary>
    /// The logger.
    /// </summary>
    private readonly ILogger logger;

    /// <summary>
    /// Creates a new instance of the medical specialties management.
    /// </summary>
    /// <param name="context">The data context.</param>
    /// <param name="logger">The logger.</param>
    public MedicalSpecialtiesManagement(IDataContext context, ILogger<MedicalSpecialtiesManagement> logger)
    {
        this.context = context;
        this.logger = logger;
    }

    /// <summary>
    /// Creates one or more medical specialties.
    /// </summary>
    /// <param name="descriptions">The medical specialties descriptions.</param>
    /// <exception cref="ValidationFailException">Thrown when the validation fails.</exception>
    /// <exception cref="ManagementFailException">Thrown when the management fails.</exception>
    public async Task CreateMedicalSpecialtiesAsync(IEnumerable<string> decriptions)
    {
        try
        {
            foreach (var description in decriptions)
            {
                if (await context.ExistsAsync<MedicalSpeciality>(i => string.Equals(i.Description, description, StringComparison.OrdinalIgnoreCase)))
                    continue;

                var model = new MedicalSpeciality(description);
                if (!IsValid(model, out var validationException))
                    throw validationException;

                await context.StoreAsync(model);
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
            var speciality = await context.FindAsync<MedicalSpeciality>(i => string.Equals(i.Description, description, StringComparison.OrdinalIgnoreCase));
            if (speciality is null)
                throw new ValidationFailException(MessageResources.MedicalSpecialitiesDescriptionNotFound);

            await context.RemoveAsync<MedicalSpeciality>(speciality.Id);

            var doctors = await context.QueryAsync<Doctor>(i => i.Specialties.Contains(description));
            foreach (var doctor in doctors)
            {
                doctor.Specialties.Remove(description);
                await context.StoreAsync<Doctor>(doctor);
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
            return await context.ToListAsync<MedicalSpeciality>();
        }
        catch (Exception ex)
        {
            logger?.Fail(ex);
            throw new ManagementFailException(MessageResources.MedicalSpecialitiesGetFail);
        }
    }
}