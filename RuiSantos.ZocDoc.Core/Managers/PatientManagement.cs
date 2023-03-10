using Microsoft.Extensions.Logging;
using RuiSantos.ZocDoc.Core.Data;
using RuiSantos.ZocDoc.Core.Managers.Exceptions;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Core.Resources;
using static RuiSantos.ZocDoc.Core.Resources.ManagementUtils;

namespace RuiSantos.ZocDoc.Core.Managers;

/// <summary>
/// This class is responsible for managing patients.
/// </summary>
public class PatientManagement : IPatientManagement
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
    /// Creates a new instance of <see cref="PatientManagement"/>.
    /// </summary>
    /// <param name="context">The data context.</param>
    /// <param name="logger">The logger.</param>
    public PatientManagement(IDataContext context, ILogger<PatientManagement> logger)
    {
        this.context = context;
        this.logger = logger;
    }

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
    public async Task CreatePatientAsync(string socialNumber, string email, string firstName, string lastName, IEnumerable<string> contactNumbers)
    {
        try
        {
            var patient = new Patient(socialNumber, email, firstName, lastName, contactNumbers);

            if (!IsValid(patient, out var validationFailException))
                throw validationFailException;

            await context.StoreAsync(patient);
        }
        catch (ValidationFailException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger?.LogException(ex);
            throw new ManagementFailException(MessageResources.PatientSetFail);
        }
    }

    /// <summary>
    /// Gets a patient by social number.
    /// </summary>
    /// <param name="socialNumber">The social security number.</param>
    /// <returns>The patient.</returns>
    /// <exception cref="ManagementFailException">If the patient could not be retrieved.</exception>
    public async Task<Patient?> GetPatientBySocialNumberAsync(string socialNumber)
    {
        try
        {
            return await context.FindAsync<Patient>(i => i.SocialSecurityNumber == socialNumber);
        }
        catch (Exception ex)
        {
            logger?.LogException(ex);
            throw new ManagementFailException(MessageResources.PatientSetFail);
        }
    }

    /// <summary>
    /// Gets all appointments for a patient.
    /// </summary>
    /// <param name="socialNumber">The social security number.</param>
    /// <returns>The appointments.</returns>
    public async IAsyncEnumerable<DoctorAppointment> GetAppointmentsAsync(string socialNumber)
    {
        var patient = await context.FindAsync<Patient>(patient => patient.SocialSecurityNumber == socialNumber);
        if (patient is null || !patient.Appointments.Any())
            yield break;

        var doctors = await context.QueryAsync<Doctor>(d => d.Appointments.Any(da => patient.Appointments.Any(pa => pa.Id == da.Id)));
        foreach (var doctor in doctors)
        {
            var dates = doctor.Appointments.Where(da => patient.Appointments.Any(pa => da.Id == pa.Id))
                .Select(da => da.GetDateTime());

            foreach (var date in dates) yield return new(doctor, date);
        }
    }
}
