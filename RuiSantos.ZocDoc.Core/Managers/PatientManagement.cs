using Microsoft.Extensions.Logging;
using RuiSantos.ZocDoc.Core.Data;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Core.Resources;
using RuiSantos.ZocDoc.Core.Validators;

namespace RuiSantos.ZocDoc.Core.Managers;

public class PatientManagement : ManagementBase
{
    private readonly IDataContext context;
    private readonly ILogger logger;

    public PatientManagement(IDataContext context, ILogger<PatientManagement> logger)
    {
        this.context = context;
        this.logger = logger;
    }

    public async Task CreatePatientAsync(string socialNumber, string email, string firstName, string lastName, IEnumerable<string> contactNumbers)
    {
        try
        {
            var patient = new Patient(socialNumber, email, firstName, lastName, contactNumbers);

            if (!IsValid(patient, PatientValidator.Instance, out var validationFailException))
                throw validationFailException;

            await context.StoreAsync(patient);
        }
        catch (ValidationFailException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger?.LogException(nameof(PatientManagement), nameof(CreatePatientAsync), ex);
            throw new ManagementFailException(MessageResources.PatientSetFail);
        }
    }

    public async Task<Patient?> GetPatientBySocialNumberAsync(string socialNumber)
    {
        try
        {
            return await context.FindAsync<Patient>(i => i.SocialSecurityNumber == socialNumber);
        }
        catch (Exception ex)
        {
            logger?.LogException(nameof(PatientManagement), nameof(GetPatientBySocialNumberAsync), ex);
            throw new ManagementFailException(MessageResources.PatientSetFail);
        }
    }

    public async IAsyncEnumerable<(Doctor doctor, DateTime date)> GetAppointmentsAsync(string socialNumber)
    {
        var patient = await context.FindAsync<Patient>(patient => patient.SocialSecurityNumber == socialNumber);
        if (patient is null || !patient.Appointments.Any())
            yield break;

        var doctors = await context.QueryAsync<Doctor>(d => d.Appointments.Any(da => patient.Appointments.Any(pa => pa.Id == da.Id)));
        foreach (var doctor in doctors)
        {
            var dates = doctor.Appointments.Where(da => patient.Appointments.Any(pa => da.Id == pa.Id))
                .Select(da => da.GetDateTime());

            foreach (var date in dates) yield return (doctor, date);
        }
    }
}
