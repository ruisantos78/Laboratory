using Microsoft.Extensions.Logging;
using RuiSantos.ZocDoc.Core.Data;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Core.Resources;
using RuiSantos.ZocDoc.Core.Validators;

namespace RuiSantos.ZocDoc.Core.Managers;

public class DoctorManagement : ManagementBase
{
    private readonly DomainContext domainContext;
    private readonly IDataContext context;
    private readonly ILogger logger;

    public DoctorManagement(DomainContext domainContext, IDataContext context, ILogger<DoctorManagement> logger)
    {
        this.domainContext = domainContext;
        this.context = context;
        this.logger = logger;
    }

    public async Task CreateDoctorAsync(string license, string email, string firstName, string lastName, 
        IEnumerable<string> contactNumbers, IEnumerable<string> specialties)
    {
        try
        {
            var model = new Doctor(Guid.NewGuid(), license, email, firstName, lastName, contactNumbers, specialties);
            await ValidateDoctorAsync(model);
            await context.StoreAsync(model);
        }
        catch (ValidationFailException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger?.LogException(nameof(DoctorManagement), nameof(CreateDoctorAsync), ex);
            throw new ManagementFailException(MessageResources.DoctorSetFail);
        }
    }

    public async Task SetOfficeHoursAsync(string license, DayOfWeek dayOfWeek, IEnumerable<TimeSpan> hours)
    {
        try
        {
            var doctor = await context.FindAsync<Doctor>(i => i.License == license);
            if (doctor is null)
                throw new ValidationFailException(MessageResources.DoctorLicenseNotFound);

            doctor.Appointments.RemoveAll(appointment => appointment.Week == dayOfWeek && !hours.Contains(appointment.Time));
            doctor.OfficeHours.RemoveAll(hour => hour.Week == dayOfWeek);
            doctor.OfficeHours.Add(new OfficeHour(dayOfWeek, hours));            

            await ValidateDoctorAsync(doctor);
            await context.StoreAsync(doctor);
        }
        catch (ValidationFailException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger?.LogException(nameof(DoctorManagement), nameof(SetOfficeHoursAsync), ex);
            throw new ManagementFailException(MessageResources.DoctorSetFail);
        }
    }

    public async Task<Doctor?> GetDoctorByLicenseAsync(string license)
    {
        try
        {
            return await context.FindAsync<Doctor>(doctor => doctor.License == license);
        }
        catch (Exception ex)
        {
            logger?.LogException(nameof(DoctorManagement), nameof(GetDoctorByLicenseAsync), ex);
            throw new ManagementFailException(MessageResources.DoctorsGetFail);
        }
    }

    public async IAsyncEnumerable<(Doctor doctor, IEnumerable<DateTime> schedule)> GetDoctorWithScheduleBySpecialityAsync(
        string speciality, DateTime dateTime)
    {
        var date = DateOnly.FromDateTime(dateTime);

        var doctors = await GetDoctorsBySpecialtyWithAvailabilityAtAsync(speciality, date);

        foreach (var doctor in doctors)
        {
            var schedule = doctor.OfficeHours
                .Where(hour => hour.Week == date.DayOfWeek)
                .SelectMany(hour => hour.Hours)
                .Except(doctor.Appointments.Where(appointment => appointment.Date == date).Select(appointment => appointment.Time))
                .Select(time => date.ToDateTime(TimeOnly.FromTimeSpan(time)));

            yield return (doctor, schedule);
        }
    }

    public async Task<List<Doctor>> GetDoctorsBySpecialtyWithAvailabilityAtAsync(string speciality, DateOnly date)
    {
        try
        {
            return await context.QueryAsync<Doctor>(doctor =>
                doctor.Specialties.Any(item => item.Equals(speciality, StringComparison.OrdinalIgnoreCase)) &&
                doctor.Appointments.Count(ap => ap.Date == date) < doctor.OfficeHours.Count(hour => hour.Week == date.DayOfWeek)
            );
        }
        catch (Exception ex)
        {
            logger?.LogException(nameof(DoctorManagement), nameof(GetDoctorByLicenseAsync), ex);
            throw new ManagementFailException(MessageResources.DoctorsGetFail);
        }
    }
    
    private async Task ValidateDoctorAsync(Doctor model)
    {
        var medicalSpecialties = await domainContext.GetMedicalSpecialtiesAsync();
        if (!IsValid(model, new DoctorValidator(medicalSpecialties), out var validationFailException))
            throw validationFailException;
    }
}
