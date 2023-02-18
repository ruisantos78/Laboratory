using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using RuiSantos.ZocDoc.Core.Data;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Core.Resources;
using RuiSantos.ZocDoc.Core.Validators;

namespace RuiSantos.ZocDoc.Core.Managers;
public class DoctorManagement : ManagementBase
{
    private readonly IDataContext context;
    private readonly IMemoryCache cache;
    private readonly ILogger logger;

    public DoctorManagement(IDataContext context, IMemoryCache cache, ILogger<DoctorManagement> logger)
    {
        this.context = context;
        this.cache = cache;
        this.logger = logger;
    }

    public async Task CreateDoctorAsync(string license, List<string> specialties,
        string email, string firstName, string lastName, List<string> contactNumbers)
    {
        try
        {
            var model = new Doctor(Guid.NewGuid(), license, specialties, email, firstName, lastName, contactNumbers);

            if (!IsValid(model, new DoctorValidator(GetMedicalSpecialties()), out var validationFailException))
                throw validationFailException;

            await context.StoreAsync(model);
        }
        catch (ValidationFailException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger?.LogException(nameof(MedicalSpecialtiesManagement), nameof(CreateDoctorAsync), ex);
            throw new ManagementFailException(MessageResources.DoctorStoreFail);
        }
    }

    public async Task<List<Doctor>> GetDoctorsAsync()
    {
        try
        {
            return await context.ListAsync<Doctor>();
        }
        catch (Exception ex)
        {
            logger?.LogException(nameof(MedicalSpecialtiesManagement), nameof(GetDoctorsAsync), ex);
            throw new ManagementFailException(MessageResources.DoctorsListFail);
        }
    }

    public async Task SetOfficeHoursAsync(Guid doctorId, DayOfWeek dayOfWeek, string[] hours)
    {
        try
        {
            var week = Enum.GetName(typeof(DayOfWeek), dayOfWeek) ?? nameof(DayOfWeek.Sunday);
            var timeHours = hours.Select(TimeSpan.Parse).ToArray();

            if (!await context.ExistsAsync<Doctor>(i => i.Id == doctorId))
                throw new ValidationFailException(MessageResources.DoctorIdNotFound);

            var schedule = await context.FindAsync<Schedule>(i => i.DoctorId == doctorId)
                ?? new Schedule() { DoctorId = doctorId };

            schedule.Appointments.RemoveAll(i => i.Date >= DateTime.Now && i.Date.DayOfWeek == dayOfWeek && !timeHours.Contains(i.Date.TimeOfDay));
            schedule.OfficeHours[week] = timeHours;

            if (!IsValid(schedule, ScheduleValidator.Instance, out var validationFailException))
                throw validationFailException;

            await context.StoreAsync(schedule);
        }
        catch (ValidationFailException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger?.LogException(nameof(MedicalSpecialtiesManagement), nameof(SetOfficeHoursAsync), ex);
            throw new ManagementFailException(MessageResources.OfficeHoursStoreFailed);
        }
    }

    private IEnumerable<MedicalSpeciality>? GetMedicalSpecialties()
    {
        return cache.GetOrCreate<IEnumerable<MedicalSpeciality>>(nameof(MedicalSpeciality), entry =>
        {
            var values = context.ListAsync<MedicalSpeciality>().Result;
            entry.SetSlidingExpiration(TimeSpan.FromMinutes(5))
                .SetValue(values);
            return values;
        });
    }
}
