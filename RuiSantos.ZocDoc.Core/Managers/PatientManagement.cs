using Microsoft.Extensions.Logging;
using RuiSantos.ZocDoc.Core.Adapters;
using RuiSantos.ZocDoc.Core.Managers.Exceptions;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Core.Resources;
using static RuiSantos.ZocDoc.Core.Resources.ManagementUtils;

namespace RuiSantos.ZocDoc.Core.Managers;

/// <summary>
/// Interface for managing patients.
/// </summary>
public interface IPatientManagement
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
    /// <exception cref="ManagementFailException">If the patient could not be created.</exception>
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
    /// <exception cref="ManagementFailException">If the patient could not be retrieved.</exception>
    Task<Patient?> GetPatientBySocialNumberAsync(string socialNumber);
}

internal class PatientManagement : IPatientManagement
{
    private readonly IPatientAdapter patientAdapter;
    private readonly IDoctorAdapter doctorAdapter;
    private readonly ILogger logger;

    public PatientManagement(IPatientAdapter patientAdapter, IDoctorAdapter doctorAdapter, ILogger<PatientManagement> logger)
    {
        this.patientAdapter = patientAdapter;
        this.doctorAdapter = doctorAdapter;
        this.logger = logger;
    }

    public async Task CreatePatientAsync(string socialNumber, string email, string firstName, string lastName, IEnumerable<string> contactNumbers)
    {
        try
        {
            var patient = new Patient()
            {
                SocialSecurityNumber = socialNumber,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                ContactNumbers = contactNumbers.ToHashSet()
            };

            ThrowExceptionIfIsNotValid(patient);
            await patientAdapter.StoreAsync(patient);
        }
        catch (ValidationFailException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger?.Fail(ex);
            throw new ManagementFailException(MessageResources.PatientSetFail);
        }
    }

    public async Task<Patient?> GetPatientBySocialNumberAsync(string socialNumber)
    {
        try
        {
            return await patientAdapter.FindAsync(socialNumber);
        }
        catch (Exception ex)
        {
            logger?.Fail(ex);
            throw new ManagementFailException(MessageResources.PatientSetFail);
        }
    }
    
    public async IAsyncEnumerable<DoctorAppointment> GetAppointmentsAsync(string socialNumber)
    {
        var patient = await patientAdapter.FindAsync(socialNumber);
        if (patient is null || !patient.Appointments.Any())
            yield break;

        var doctors = await doctorAdapter.FindAllWithAppointmentsAsync(patient.Appointments);
        foreach (var doctor in doctors)
        {
            var dates = doctor.Appointments.Where(da => patient.Appointments.Any(pa => da.Id == pa.Id))
                .Select(da => da.GetDateTime());

            foreach (var date in dates) yield return new(doctor, date);
        }
    }
}
