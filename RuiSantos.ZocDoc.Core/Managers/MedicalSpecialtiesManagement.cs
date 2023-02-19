using Microsoft.Extensions.Logging;
using RuiSantos.ZocDoc.Core.Data;
using RuiSantos.ZocDoc.Core.Managers.Exceptions;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Core.Resources;
using RuiSantos.ZocDoc.Core.Validators;

namespace RuiSantos.ZocDoc.Core.Managers;

public class MedicalSpecialtiesManagement : ManagementBase
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
                if (!IsValid(model, MedicalSpecialityValidator.Instance, out var validationException))
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
            logger?.LogException(nameof(MedicalSpecialtiesManagement), nameof(CreateMedicalSpecialtiesAsync), ex);
            throw new ManagementFailException(MessageResources.MedicalSpecialitiesSetFail);
        }
    }

    public async Task RemoveMedicalSpecialtiesAsync(string description)
    {
        try
        {
            var query = await context.QueryAsync<MedicalSpeciality>(i => string.Equals(i.Description, description, StringComparison.OrdinalIgnoreCase));

            if (query.FirstOrDefault() is not MedicalSpeciality model)
                throw new ValidationFailException(MessageResources.MedicalSpecialitiesDescriptionNotFound);

            await context.RemoveAsync<MedicalSpeciality>(model.Id);
        }
        catch (ValidationFailException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger?.LogException(nameof(MedicalSpecialtiesManagement), nameof(RemoveMedicalSpecialtiesAsync), ex);
            throw new ManagementFailException(MessageResources.MedicalSpecialitiesSetFail);
        }
    }

    public async Task<List<MedicalSpeciality>> GetMedicalSpecialitiesAsync()
    {
        try
        {
            return await context.ListAsync<MedicalSpeciality>();
        }
        catch (Exception ex)
        {
            logger?.LogException(nameof(MedicalSpecialtiesManagement), nameof(GetMedicalSpecialitiesAsync), ex);
            throw new ManagementFailException(MessageResources.MedicalSpecialitiesGetFail);
        }
    }
}