using Microsoft.Extensions.Logging;
using RuiSantos.Labs.Core.Extensions;
using RuiSantos.Labs.Core.Repositories;
using RuiSantos.Labs.Core.Services.Exceptions;
using RuiSantos.Labs.Core.Resources;

namespace RuiSantos.Labs.Core.Services;

/// <summary>
/// Interface for managing medical specialties.
/// </summary>
public interface IMedicalSpecialtiesService
{
    /// <summary>
    /// Creates a new medical specialty.
    /// </summary>
    /// <param name="descriptions">The description of the specialty.</param>
    /// <exception cref="ValidationFailException">Thrown when the validation fails.</exception>
    /// <exception cref="ServiceFailException">Thrown when the management fails.</exception>
    Task CreateMedicalSpecialtiesAsync(List<string> descriptions);

    /// <summary>
    /// Gets all the medical specialties.
    /// </summary>
    /// <returns>A list of medical specialties.</returns>
    /// <exception cref="ServiceFailException">Thrown when the management fails.</exception>
    Task<IReadOnlySet<string>> GetMedicalSpecialitiesAsync();

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
    private readonly IMedicalSpecialityRepository _medicalSpecialityRepository;
    private readonly ILogger _logger;

    public MedicalSpecialtiesService(
        IMedicalSpecialityRepository medicalSpecialityRepository,   
        ILogger<MedicalSpecialtiesService> logger)
    {
        _medicalSpecialityRepository = medicalSpecialityRepository;
        _logger = logger;
    }

    public async Task CreateMedicalSpecialtiesAsync(List<string> descriptions)
    {
        try
        {
            await _medicalSpecialityRepository.AddAsync(descriptions);
        }
        catch (Exception ex)
        {
            _logger.Fail(ex);
            throw new ServiceFailException(MessageResources.MedicalSpecialitiesSetFail);
        }
    }

    public async Task RemoveMedicalSpecialtiesAsync(string description)
    {
        try
        {
            await _medicalSpecialityRepository.RemoveAsync(description);
        }
        catch (Exception ex)
        {
            _logger.Fail(ex);
            throw new ServiceFailException(MessageResources.MedicalSpecialitiesSetFail);
        }
    }

    public async Task<IReadOnlySet<string>> GetMedicalSpecialitiesAsync()
    {
        try
        {
            return await _medicalSpecialityRepository.GetAsync();
        }
        catch (Exception ex)
        {
            _logger.Fail(ex);
            throw new ServiceFailException(MessageResources.MedicalSpecialitiesGetFail);
        }
    }
}