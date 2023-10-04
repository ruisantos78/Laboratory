using Amazon.DynamoDBv2;
using RuiSantos.Labs.Core.Repositories;
using RuiSantos.Labs.Core.Models;
using RuiSantos.Labs.Data.Dynamodb.Adapters;

namespace RuiSantos.Labs.Data.Dynamodb.Repositories;

public class DoctorRepository : IDoctorRepository
{
    private readonly DoctorAdapter _doctorAdapter;
    private readonly Lazy<AppointmentAdapter> _appointmentAdapter;

    public DoctorRepository(IAmazonDynamoDB client)
    {
        _doctorAdapter = new DoctorAdapter(client);
        _appointmentAdapter = new Lazy<AppointmentAdapter>(new AppointmentAdapter(client));
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
        var appointmentAdapter = _appointmentAdapter.Value;            

        await foreach (var doctor in _doctorAdapter.LoadBySpecialtyAsync(specialty))
        {
            if (doctor.OfficeHours.All(h => h.Week != date.DayOfWeek))
                continue;
                                    
            await foreach (var appointment in appointmentAdapter.LoadByDoctorAsync(doctor, date)) 
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

    public IAsyncEnumerable<Doctor> FindByAppointmentsAsync(IEnumerable<Appointment> appointments)
    {
        return appointments
            .Select(x => _doctorAdapter.FindByAppointmentAsync(x))
            .OfType<Task<Doctor>>()
            .AsModelsAsyncEnumerable();
    }

    public Task StoreAsync(Doctor doctor)
    {
        return _doctorAdapter.StoreAsync(doctor);
    }

    public IAsyncEnumerable<Doctor> FindAllAsync(int take, string? lastLicense = null)
    {
        return _doctorAdapter.LoadByLicenseAsync(lastLicense, take);
    }

    public Task<Doctor?> FindByLicenseAsync(string license)
    {
        return _doctorAdapter.FindByLicenseAsync(license);
    }
}