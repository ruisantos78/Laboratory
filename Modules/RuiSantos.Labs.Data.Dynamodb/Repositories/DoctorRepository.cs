using RuiSantos.Labs.Core.Repositories;
using RuiSantos.Labs.Core.Models;
using RuiSantos.Labs.Data.Dynamodb.Adapters;

namespace RuiSantos.Labs.Data.Dynamodb.Repositories;

public class DoctorRepository : IDoctorRepository
{
    private readonly IDoctorAdapter _doctorAdapter;
    private readonly IAppointmentAdapter _appointmentAdapter;

    public DoctorRepository(
        IDoctorAdapter doctorAdapter,
        IAppointmentAdapter appointmentAdapter)
    {
        _doctorAdapter = doctorAdapter;
        _appointmentAdapter = appointmentAdapter;
    }

    public Task<Doctor?> FindAsync(Guid doctorId)
    {
        return _doctorAdapter.FindAsync(doctorId);
    }

    public Task<IEnumerable<Doctor>> FindBySpecialityAsync(string specialty)
    {
        return _doctorAdapter.LoadBySpecialtyAsync(specialty);
    }

    public async IAsyncEnumerable<DoctorSchedule> FindBySpecialtyWithAvailabilityAsync(string specialty, DateOnly date)
    {
        foreach (var doctor in await _doctorAdapter.LoadBySpecialtyAsync(specialty))
        {
            if (doctor.OfficeHours.All(h => h.Week != date.DayOfWeek))
                continue;

            foreach (var appointment in await _appointmentAdapter.LoadByDoctorAsync(doctor, date))
            {
                var availableTimes = doctor.OfficeHours
                    .Where(x => x.Week == date.DayOfWeek)
                    .SelectMany(s => s.Hours.Where(hour => hour != appointment.Time))
                    .Select(x => date.ToDateTime(TimeOnly.FromTimeSpan(x)))
                    .ToList();

                if (availableTimes.Any())
                    yield return new(doctor, availableTimes);
            }
        }
    }

    public async Task<IEnumerable<Doctor>> FindByAppointmentsAsync(IEnumerable<Appointment> appointments)
    {
        var tasks = appointments.Select(_doctorAdapter.FindByAppointmentAsync)
            .OfType<Task<Doctor>>();

        return await Task.WhenAll(tasks);
    }

    public Task StoreAsync(Doctor doctor)
    {
        return _doctorAdapter.StoreAsync(doctor);
    }

    public Task<Pagination<Doctor>> FindAllAsync(int take, string? paginationToken)
    {
        return _doctorAdapter.LoadByLicenseAsync(take, paginationToken);
    }

    public Task<Doctor?> FindByLicenseAsync(string license)
    {
        return _doctorAdapter.FindByLicenseAsync(license);
    }
}