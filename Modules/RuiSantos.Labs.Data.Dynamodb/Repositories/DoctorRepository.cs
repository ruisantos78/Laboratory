using Amazon.DynamoDBv2;
using RuiSantos.Labs.Core.Repositories;
using RuiSantos.Labs.Core.Models;
using RuiSantos.Labs.Data.Dynamodb.Adapters;

namespace RuiSantos.Labs.Data.Dynamodb.Repositories;

public class DoctorRepository : IDoctorRepository
{
    private readonly DoctorAdapter doctorAdapter;
    private readonly IAmazonDynamoDB client;

    public DoctorRepository(IAmazonDynamoDB client)
    {
        this.doctorAdapter = new DoctorAdapter(client);
        this.client = client;
    }

    public async Task<Doctor?> FindAsync(Guid doctorId)
    {
        return await doctorAdapter.FindAsync(doctorId);
    }

    public IAsyncEnumerable<Doctor> FindBySpecialityAsync(string specialty)
    {
        return doctorAdapter.LoadBySpecialtyAsync(specialty);
    }

    public async IAsyncEnumerable<DoctorSchedule> FindBySpecialtyWithAvailabilityAsync(string specialty, DateOnly date)
    {
        var appointmentAdapter = new AppointmentAdapter(client);            

        await foreach (var doctor in doctorAdapter.LoadBySpecialtyAsync(specialty)) 
        {
            if (!doctor.OfficeHours.Any(h => h.Week == date.DayOfWeek))
                continue;
                                    
            await foreach (var appointment in appointmentAdapter.LoadByDoctorAsync(doctor, date)) 
            {
                 var availableTimes = doctor.OfficeHours
                    .Where(x => x.Week == date.DayOfWeek)
                    .SelectMany(s => s.Hours.Where(hour => hour != appointment.Time))
                    .Select(x => date.ToDateTime(TimeOnly.FromTimeSpan(x)));
                                  
                if (availableTimes?.Any() is true)
                    yield return new(doctor, availableTimes);
            }
        }                          
    }

    public IAsyncEnumerable<Doctor> FindByAppointmentsAsync(IEnumerable<Appointment> appointments)
    {
        return appointments
            .Select(x => doctorAdapter.FindByAppointmentAsync(x))
            .OfType<Task<Doctor>>()
            .ToModelsAsync();      
    }

    public async Task StoreAsync(Doctor doctor)
    {
        await doctorAdapter.StoreAsync(doctor);
    }

    public IAsyncEnumerable<Doctor> FindAllAsync(int take, string? lastLicense = null)
    {
        return doctorAdapter.LoadByLicenseAsync(lastLicense, take);
    }

    public Task<Doctor?> FindByLicenseAsync(string license)
    {
        return doctorAdapter.FindByLicenseAsync(license);
    }
}