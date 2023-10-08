using RuiSantos.Labs.Core.Repositories;
using RuiSantos.Labs.Core.Models;
using RuiSantos.Labs.Data.Dynamodb.Adapters;

namespace RuiSantos.Labs.Data.Dynamodb.Repositories;

public class DoctorRepository : IDoctorRepository
{
    private readonly IDoctorAdapter _doctorAdapter;
    private readonly IAppointmentAdapter _appointmentAdapter;

    internal DoctorRepository(
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

    public IAsyncEnumerable<Doctor> FindBySpecialityAsync(string specialty)
    {
        return _doctorAdapter.LoadBySpecialtyAsync(specialty);
    }

    public async IAsyncEnumerable<DoctorSchedule> FindBySpecialtyWithAvailabilityAsync(string specialty, DateOnly date)
    {
        await foreach (var doctor in _doctorAdapter.LoadBySpecialtyAsync(specialty))
        {
            if (doctor.OfficeHours.All(h => h.Week != date.DayOfWeek))
                continue;
                                    
            await foreach (var appointment in _appointmentAdapter.LoadByDoctorAsync(doctor, date)) 
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

    public async IAsyncEnumerable<Doctor> FindByAppointmentsAsync(IEnumerable<Appointment> appointments)
    {
        var tasks = appointments.Select(_doctorAdapter.FindByAppointmentAsync)
            .OfType<Task<Doctor>>();

        foreach (var task in tasks)
            yield return await task;
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