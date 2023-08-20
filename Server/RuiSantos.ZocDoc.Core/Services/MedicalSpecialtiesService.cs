using Microsoft.Extensions.Logging;
using RuiSantos.ZocDoc.Core.Repositories;
using RuiSantos.ZocDoc.Core.Services.Exceptions;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Core.Resources;
using RuiSantos.ZocDoc.Core.Validators;

namespace RuiSantos.ZocDoc.Core.Services;

/// <summary>
/// Interface for managing medical specialties.
/// </summary>
public interface IMedicalSpecialtiesService
{
    /// <summary>
    /// Creates a new medical specialty.
    /// </summary>
    /// <param name="decriptions">The description of the specialty.</param>
    /// <exception cref="ValidationFailException">Thrown when the validation fails.</exception>
    /// <exception cref="ServiceFailException">Thrown when the management fails.</exception>
    Task CreateMedicalSpecialtiesAsync(List<string> decriptions);

    /// <summary>
    /// Gets all the medical specialties.
    /// </summary>
    /// <returns>A list of medical specialties.</returns>
    /// <exception cref="ServiceFailException">Thrown when the management fails.</exception>
    Task<List<MedicalSpecialty>> GetMedicalSpecialitiesAsync();

    /// <summary>
    /// Removes a medical specialty.
    /// </summary>
    /// <param name="description">The description of the specialty.</param>
    /// <exception cref="ValidationFailException">Thrown when the validation fails.</exception>
    /// <exception cref="ServiceFailException">Thrown when the management fails.</exception>
    Task RemoveMedicalSpecialtiesAsync(string description);
}

internal class MedicalSpecialtiesService : IMedicalSpecialtiesService
{
    private readonly IMedicalSpecialityRepository medicalSpecialityRepository;
    private readonly IDoctorRepository doctorRepository;
    private readonly ILogger logger;

    public MedicalSpecialtiesService(IMedicalSpecialityRepository medicalSpecialityRepository,
                                        IDoctorRepository doctorRepository,
                                        ILogger<MedicalSpecialtiesService> logger)
    {
        this.medicalSpecialityRepository = medicalSpecialityRepository;
        this.doctorRepository = doctorRepository;
        this.logger = logger;
    }

    public async Task CreateMedicalSpecialtiesAsync(List<string> decriptions)
    {
        try
        {
            foreach (var description in decriptions)
            {
                if (await medicalSpecialityRepository.ContainsAsync(description))
                    continue;

                var model = new MedicalSpecialty(description);

                Validator.ThrowExceptionIfIsNotValid(model);
                await medicalSpecialityRepository.AddAsync(model);
            }
            
        }
        catch (ValidationFailException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger?.Fail(ex);
            throw new ServiceFailException(MessageResources.MedicalSpecialitiesSetFail);
        }
    }

    public async Task RemoveMedicalSpecialtiesAsync(string description)
    {
        try
        {
            if (await medicalSpecialityRepository.ContainsAsync(description) == false) 
                throw new ValidationFailException(MessageResources.MedicalSpecialitiesDescriptionNotFound);

            await medicalSpecialityRepository.RemoveAsync(description);

            var doctors = await doctorRepository.FindBySpecialityAsync(description);
            foreach (var doctor in doctors)
            {
                doctor.Specialties.Remove(description);
                await doctorRepository.StoreAsync(doctor);
            }            
        }
        catch (ValidationFailException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger?.Fail(ex);
            throw new ServiceFailException(MessageResources.MedicalSpecialitiesSetFail);
        }
    }

    public async Task<List<MedicalSpecialty>> GetMedicalSpecialitiesAsync()
    {
        try
        {
            return await medicalSpecialityRepository.ToListAsync();
        }
        catch (Exception ex)
        {
            logger?.Fail(ex);
            throw new ServiceFailException(MessageResources.MedicalSpecialitiesGetFail);
        }
    }
}