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
            var doctor = new Doctor(license, email, firstName, lastName, contactNumbers, specialties);
            await ValidateDoctorAsync(doctor);
            await context.StoreAsync(doctor);
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

            doctor.OfficeHours.RemoveAll(hour => hour.Week == dayOfWeek);
            if (hours.Any())
                doctor.OfficeHours.Add(new OfficeHour(dayOfWeek, hours));

            await StoreAndCancelAppointmentsAsync(doctor, dayOfWeek, hours);
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

    public async IAsyncEnumerable<(Patient patient, DateTime date)> GetAppointmentsAsync(string license, DateTime? dateTime)
    {
        var date = DateOnly.FromDateTime(dateTime ?? DateTime.Today);

        var doctor = await context.FindAsync<Doctor>(doctor => doctor.License == license && doctor.Appointments.Any(da => da.Date == date));
        if (doctor is null)
            yield break;

        var appointments = doctor.Appointments.FindAll(a => a.Date == date);

        var patients = await context.QueryAsync<Patient>(p => p.Appointments.Any(pa => appointments.Any(da => da.Id == pa.Id)));
        foreach (var patient in patients)
        {
            var dates = patient.Appointments.Where(pa => appointments.Any(da => da.Id == pa.Id))
                .Select(pa => pa.GetDateTime());

            foreach (var appointmentDate in dates) yield return (patient, appointmentDate);
        }
    }

    private async Task ValidateDoctorAsync(Doctor model)
    {
        var medicalSpecialties = await domainContext.GetMedicalSpecialtiesAsync();
        if (!IsValid(model, new DoctorValidator(medicalSpecialties), out var validationFailException))
            throw validationFailException;
    }

    private async Task StoreAndCancelAppointmentsAsync(Doctor doctor, DayOfWeek dayOfWeek, IEnumerable<TimeSpan> hours)
    {
        var appointments = doctor.Appointments.FindAll(appointment => appointment.Week == dayOfWeek && !hours.Contains(appointment.Time));

        var patients = await context.QueryAsync<Patient>(patient => patient.Appointments.Any(item => appointments.Any(a => a.Id == item.Id)));
        foreach (var patient in patients)
        {
            patient.Appointments.RemoveAll(item => appointments.Any(a => a.Id == item.Id));
            await context.StoreAsync(patient);
        }

        doctor.Appointments.RemoveAll(item => appointments.Any(a => a.Id == item.Id));
        await context.StoreAsync(doctor);
    }
}
