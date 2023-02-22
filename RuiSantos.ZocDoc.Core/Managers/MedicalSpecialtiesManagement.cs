using Microsoft.Extensions.Logging;
using RuiSantos.ZocDoc.Core.Data;
using RuiSantos.ZocDoc.Core.Managers.Exceptions;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Core.Resources;
using static RuiSantos.ZocDoc.Core.Resources.ManagementUtils;

namespace RuiSantos.ZocDoc.Core.Managers;

public class MedicalSpecialtiesManagement
{
    private readonly IDataContext context;
    private readonly ILogger logger;

    public MedicalSpecialtiesManagement(IDataContext context, ILogger<MedicalSpecialtiesManagement> logger)
    {
        this.context = context;
        this.logger = logger;
    }

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
            logger?.LogException(ex);
            throw new ManagementFailException(MessageResources.MedicalSpecialitiesSetFail);
        }
    }

    public async Task RemoveMedicalSpecialtiesAsync(string description)
    {
        try
        {
            var speciality = await context.FindAsync<MedicalSpeciality>(i => string.Equals(i.Description, description, StringComparison.OrdinalIgnoreCase));
            if (speciality is null)
                throw new ValidationFailException(MessageResources.MedicalSpecialitiesDescriptionNotFound);

            await context.RemoveAsync<MedicalSpeciality>(speciality.Id);

            var doctors = await context.QueryAsync<Doctor>(i => i.Specialties.Contains(description));
            foreach ( var doctor in doctors)
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
            logger?.LogException(ex);
            throw new ManagementFailException(MessageResources.MedicalSpecialitiesSetFail);
        }
    }

    public async Task<List<MedicalSpeciality>> GetMedicalSpecialitiesAsync()
    {
        try
        {
            return await context.ToListAsync<MedicalSpeciality>();
        }
        catch (Exception ex)
        {
            logger?.LogException(ex);
            throw new ManagementFailException(MessageResources.MedicalSpecialitiesGetFail);
        }
    }
}