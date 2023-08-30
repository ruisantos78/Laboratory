using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using RuiSantos.Labs.Core.Repositories;
using RuiSantos.Labs.Core.Models;
using RuiSantos.Labs.Data.Dynamodb.Entities;

namespace RuiSantos.Labs.Data.Dynamodb.Repositories;

public class DoctorRepository : IDoctorRepository
{
    private readonly IDynamoDBContext context;

    public DoctorRepository(IAmazonDynamoDB client)
    {
        this.context = new DynamoDBContext(client);
    }

    public async Task<Doctor?> FindAsync(string license)
    {
        return await DoctorDto.GetDoctorByLicenseAsync(context, license);
    }

    public async Task<List<Doctor>> FindBySpecialityAsync(string specialty)
    {
        return await DoctorDto.GetDoctorsBySpecialtyAsync(context, specialty);
    }

    public async IAsyncEnumerable<DoctorSchedule> FindBySpecialtyWithAvailabilityAsync(string specialty, DateOnly date)
    {
        var doctors = await FindBySpecialityAsync(specialty);
        if (doctors?.Any() is not true)
            yield break;

        doctors = doctors.FindAll(d => d.OfficeHours.Any(h => h.Week == date.DayOfWeek));
        if (doctors?.Any() is not true)
            yield break;

        foreach (var doctor in doctors) {
            var appointments = await AppointmentsDto.GetAppointmentsByDoctorAsync(context, doctor, date);  

            var availableTimes = doctor.OfficeHours
                .Where(x => x.Week == date.DayOfWeek)
                .SelectMany(s => s.Hours)
                .Except(appointments.Select(a => a.Time))
                .Select(hour => date.ToDateTime(TimeOnly.FromTimeSpan(hour)));
            
            if (availableTimes.Any())
                yield return new(doctor, availableTimes);
        }
    }

    public async Task<List<Doctor>> FindByAppointmentsAsync(IEnumerable<Appointment> appointments)
    {
        var tasks = appointments.Select(appoint => DoctorDto.GetDoctorByAppointmentAsync(context, appoint));
        var result = await Task.WhenAll(tasks);
        if (result?.Any(doctor => doctor is not null) is not true)
            return new List<Doctor>();

        return result.Where(doctor => doctor is not null)
            .Select(doctor => doctor!)
            .ToList();
    }

    public async Task StoreAsync(Doctor doctor)
    {
        await DoctorDto.SetDoctorAsync(context, doctor);
    }
}