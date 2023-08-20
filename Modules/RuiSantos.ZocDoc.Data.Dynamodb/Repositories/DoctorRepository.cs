using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using RuiSantos.ZocDoc.Core.Repositories;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Data.Dynamodb.Entities;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Repositories;

public class DoctorRepository : IDoctorRepository
{
    private readonly IDynamoDBContext context;

    public DoctorRepository(IAmazonDynamoDB client)
    {
        this.context = new DynamoDBContext(client);
    }

    public async Task<Doctor?> FindAsync(string license)
        => await DoctorDto.GetDoctorByLicenseAsync(context, license);
    
    public async Task<List<Doctor>> FindBySpecialityAsync(string specialty)
        => await DoctorDto.GetDoctorsBySpecialtyAsync(context, specialty);
    
    public async Task<List<Doctor>> FindBySpecialtyWithAvailabilityAsync(string specialty, DateOnly date)
    {
        var doctors = await FindBySpecialityAsync(specialty);

        return doctors
            .Select(x => new
            {
                Doctor = x,
                Schedule = x.OfficeHours.Where(h => h.Week == date.DayOfWeek).SelectMany(s => s.Hours),
                Appointments = x.Appointments.Where(a => a.Date == date).Select(s => s.Time)
            })
            .Where(x => x.Schedule.Any(s => !x.Appointments.Contains(s)))
            .Select(x => x.Doctor)
            .ToList();
    }

    public async Task<List<Doctor>> FindAllWithAppointmentsAsync(IEnumerable<Appointment> appointments)
    {
        var query = appointments.Select(async appoint => await DoctorDto.GetDoctorByAppointmentIdAsync(context, appoint.Id));        
        var queryResult = await Task.WhenAll(query);
        
        var doctors = queryResult?.Where(doctor => doctor is not null).Select(doctor => doctor!);
        return doctors?.ToList() ?? new List<Doctor>();    
    }

    public async Task StoreAsync(Doctor doctor)
    {
        await DoctorDto.SetDoctorAsync(context, doctor);
    }
}