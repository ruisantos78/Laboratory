using Microsoft.Extensions.Logging;
using RuiSantos.Labs.Core.Extensions;
using RuiSantos.Labs.Core.Repositories;
using RuiSantos.Labs.Core.Services.Exceptions;
using RuiSantos.Labs.Core.Models;
using RuiSantos.Labs.Core.Resources;
using RuiSantos.Labs.Core.Validators;

namespace RuiSantos.Labs.Core.Services;

/// <summary>
/// Interface for managing patients.
/// </summary>
public interface IPatientService
{
    /// <summary>
    /// Creates a new patient.
    /// </summary>
    /// <param name="socialNumber">The social security number.</param>
    /// <param name="email">The email.</param>
    /// <param name="firstName">The first name.</param>
    /// <param name="lastName">The last name.</param>
    /// <param name="contactNumbers">The contact numbers.</param>
    /// <exception cref="ValidationFailException">If the patient is not valid.</exception>
    /// <exception cref="ServiceFailException">If the patient could not be created.</exception>
    Task CreatePatientAsync(string socialNumber, string email, string firstName, string lastName, IEnumerable<string> contactNumbers);

    /// <summary>
    /// Gets all the appointments of a patient.
    /// </summary>
    /// <param name="socialNumber">Social number of the patient.</param>
    /// <returns>List of appointments.</returns>
    IAsyncEnumerable<DoctorAppointment> GetAppointmentsAsync(string socialNumber);

    /// <summary>
    /// Gets a patient by social number.
    /// </summary>
    /// <param name="socialNumber">The social security number.</param>
    /// <returns>The patient.</returns>
    /// <exception cref="ServiceFailException">If the patient could not be retrieved.</exception>
    Task<Patient?> GetPatientBySocialNumberAsync(string socialNumber);
}

internal class PatientService : IPatientService
{
    private readonly IPatientRepository _patientRepository;
    private readonly ILogger _logger;

    public PatientService(IPatientRepository patientRepository, ILogger<PatientService> logger)
    {
        _patientRepository = patientRepository;
        _logger = logger;
    }

    public async Task CreatePatientAsync(string socialNumber, string email, string firstName, string lastName, IEnumerable<string> contactNumbers)
    {
        try
        {
            var patient = new Patient
            {
                SocialSecurityNumber = socialNumber,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                ContactNumbers = contactNumbers.ToHashSet()
            };

            Validator.ThrowExceptionIfIsNotValid(patient);
            await _patientRepository.StoreAsync(patient);
        }
        catch (ValidationFailException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.Fail(ex);
            throw new ServiceFailException(MessageResources.PatientSetFail);
        }
    }

    public async Task<Patient?> GetPatientBySocialNumberAsync(string socialNumber)
    {
        try
        {
            return await _patientRepository.FindAsync(socialNumber);
        }
        catch (Exception ex)
        {
            _logger.Fail(ex);
            throw new ServiceFailException(MessageResources.PatientSetFail);
        }
    }
    
    public IAsyncEnumerable<DoctorAppointment> GetAppointmentsAsync(string socialNumber)
    {
        // var patient = await patientRepository.FindAsync(socialNumber);
        // if (patient is null)
        //     yield break;

        // var doctors = await doctorRepository.FindByAppointmentsAsync(patient.Appointments);
        // foreach (var doctor in doctors)
        // {
        //     var dates = doctor.Appointments.Where(da => patient.Appointments.Any(pa => da.Id == pa.Id))
        //         .Select(da => da.GetDateTime());

        //     foreach (var date in dates) yield return new(doctor, date);
        // }
        throw new NotImplementedException();
    }
}
