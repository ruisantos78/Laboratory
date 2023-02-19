using Microsoft.Extensions.Logging;
using RuiSantos.ZocDoc.Core.Data;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Core.Resources;
using RuiSantos.ZocDoc.Core.Validators;

namespace RuiSantos.ZocDoc.Core.Managers;

public class PatientManagement: ManagementBase
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
            var model = new Patient(Guid.NewGuid(), socialNumber, email, firstName, lastName, contactNumbers);

            if (!IsValid(model, PatientValidator.Instance, out var validationFailException))
                throw validationFailException;

            await context.StoreAsync(model);
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
}
