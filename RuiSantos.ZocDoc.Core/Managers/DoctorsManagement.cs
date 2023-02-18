using Microsoft.Extensions.Logging;
using RuiSantos.ZocDoc.Core.Data;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Core.Resources;
using RuiSantos.ZocDoc.Core.Validators;

namespace RuiSantos.ZocDoc.Core.Managers;

public class DoctorsManagement : ManagementBase
{
    public DoctorsManagement(IDataContext context) : base(context) { }
    public DoctorsManagement(IDataContext context, ILogger logger) : base(context, logger) { }

    public Task StoreDoctorAsync(Doctor model)
    {
        try
        {
            if (!IsValid(model, DoctorValidator.Instance, out var validationException))
                return Task.FromException(validationException);

            return context.StoreAsync(model);
        }
        catch (Exception ex)
        {
            logger?.LogException(nameof(DoctorsManagement), nameof(StoreDoctorAsync), ex);
            return ManagementFailException.FromException(MessageResources.DoctorStoreFail);
        }
    }

    public IReadOnlyList<Doctor> GetDoctorBySpecialitiesAsync(string specialty)
    {
        try
        {
            var query = from doctor in context.Query<Doctor>()
                        where
                            doctor.Specialities.Any(ms => ms.Description.Contains(specialty, StringComparison.OrdinalIgnoreCase))
                        orderby
                            doctor.LastName, doctor.FirstName
                        select
                            doctor;

            return query.ToArray();
        }
        catch (Exception ex)
        {
            logger?.LogException(nameof(DoctorsManagement), nameof(GetDoctorBySpecialitiesAsync), ex);
            throw new ManagementFailException(MessageResources.MedicalSpecialitiesListFail);
        }
    }
}