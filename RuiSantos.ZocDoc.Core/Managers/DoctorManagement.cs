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
            logger?.LogException(nameof(DoctorManagement), nameof(CreateDoctorAsync), ex);
            throw new ManagementFailException(MessageResources.DoctorStoreFail);
        }
    }

    public async Task<Doctor?> GetDoctorByLicenseAsync(string license)
    {
        try
        {
            return await context.FindAsync<Doctor>(i => i.License == license);
        }
        catch (Exception ex)
        {
            logger?.LogException(nameof(DoctorManagement), nameof(GetDoctorByLicenseAsync), ex);
            throw new ManagementFailException(MessageResources.DoctorsListFail);
        }
    }

    public async Task<List<Doctor>> GetDoctorBySpecialityAsync(string speciality, DateTime dateTime)
    {
        try
        {
            var date = DateOnly.FromDateTime(dateTime);

            return await context.QueryAsync<Doctor>(dr => 
                dr.Specialties.Any(s => s.Equals(speciality, StringComparison.OrdinalIgnoreCase))
                && dr.Appointments.Count(ap => ap.Date == date) < dr.OfficeHours.Count(oh => oh.Week == date.DayOfWeek)
            );
        }
        catch (Exception ex)
        {
            logger?.LogException(nameof(DoctorManagement), nameof(GetDoctorByLicenseAsync), ex);
            throw new ManagementFailException(MessageResources.DoctorsListFail);
        }
    }

    public async Task SetOfficeHoursAsync(string license, DayOfWeek dayOfWeek, string[] hours)
    {
        try
        {
            var timeHours = hours.Select(TimeSpan.Parse).ToList();

            var doctor = await context.FindAsync<Doctor>(i => i.License == license);
            if (doctor is null)
                throw new ValidationFailException(MessageResources.DoctorLicenseNotFound);

            doctor.Appointments.RemoveAll(app => app.Week == dayOfWeek && !timeHours.Contains(app.Time));
            doctor.OfficeHours.RemoveAll(hou => hou.Week == dayOfWeek);

            doctor.OfficeHours.Add(new OfficeHour
            {
                Week = dayOfWeek,
                Hours = timeHours
            });

            if (!IsValid(doctor, new DoctorValidator(GetMedicalSpecialties()), out var validationFailException))
                throw validationFailException;

            await context.StoreAsync(doctor);
        }
        catch (ValidationFailException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger?.LogException(nameof(DoctorManagement), nameof(SetOfficeHoursAsync), ex);
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
