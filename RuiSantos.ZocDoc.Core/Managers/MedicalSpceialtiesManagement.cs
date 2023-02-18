using Microsoft.Extensions.Logging;
using RuiSantos.ZocDoc.Core.Data;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Core.Resources;
using RuiSantos.ZocDoc.Core.Validators;

namespace RuiSantos.ZocDoc.Core.Managers;

public class MedicalSpceialtiesManagement : ManagementBase
{    
    public MedicalSpceialtiesManagement(IDataContext context) : base(context) { }
    public MedicalSpceialtiesManagement(IDataContext context, ILogger logger) : base(context, logger) { }

    public Task CreateMedicalSpecialtiesAsync(string decription)
    {
        try
        {
            var model = new MedicalSpeciality { Description = decription };
            if (!IsValid(model, MedicalSpecialityValidator.Instance, out var  validationException))
                return Task.FromException(validationException);

            return context.StoreAsync(model);
        }
        catch (Exception ex)
        {
            logger?.LogException(nameof(MedicalSpceialtiesManagement), nameof(CreateMedicalSpecialtiesAsync), ex);
            return ManagementFailException.FromException(MessageResources.MedicalSpecialitiesStoreFail);
        }
    }

    public Task RemoveMedicalSpecialtiesAsync(string id)
    {
        try
        {
            return context.RemoveAsync<MedicalSpeciality>(id);
        }
        catch (Exception ex)
        {
            logger?.LogException(nameof(MedicalSpceialtiesManagement), nameof(RemoveMedicalSpecialtiesAsync), ex);
            return ManagementFailException.FromException(MessageResources.MedicalSpecialitiesStoreFail);
        }
    }

    public IReadOnlyList<MedicalSpeciality> GetMedicalSpecialities()
    {
        try
        {
            return context.Query<MedicalSpeciality>().ToArray();
        }
        catch (Exception ex)
        {
            logger?.LogException(nameof(MedicalSpceialtiesManagement), nameof(GetMedicalSpecialities), ex);
            throw new ManagementFailException(MessageResources.MedicalSpecialitiesListFail);
        }
    }    
}