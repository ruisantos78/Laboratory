using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using RuiSantos.ZocDoc.Core.Adapters;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Data.Dynamodb.Entities;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Adapters;

public class DoctorAdapter : IDoctorAdapter
{
    private readonly IDynamoDBContext context;

    public DoctorAdapter(AmazonDynamoDBClient client)
    {
        this.context = new DynamoDBContext(client);
    }

    public async Task<Doctor?> FindAsync(string license)
    {
        return await DoctorDto.GetDoctorByLicenseAsync(context, license);
    }

    public async Task<List<Doctor>> FindBySpecialityAsync(string specialty)
    {
        var doctors = await DoctorSpecialtyDto.GetDoctorsBySpecialtyAsync(context, specialty);

        var reader = context.CreateBatchGet<DoctorDto>();
        doctors.ToList().ForEach(id => reader.AddKey(id));
        await reader.ExecuteAsync();

        return await DoctorDto.GetDoctorsAsync(context, reader.Results).ToListAsync();
    }

    public async Task<List<Doctor>> FindBySpecialtyWithAvailabilityAsync(string specialty, DateOnly date)
    {
        var doctors = await FindBySpecialityAsync(specialty);

        return doctors.FindAll(d =>
            d.Appointments.Any(a => a.Date == date) &&
            d.Appointments.Count(a => a.Date == date) < d.OfficeHours.FirstOrDefault(of => of.Week == date.DayOfWeek)?.Hours.Count());
    }

    public async Task<List<Doctor>> FindAllWithAppointmentsAsync(IEnumerable<Appointment> appointments)
    {
        var appointmentReader = context.CreateBatchGet<AppointmentsDto>();
        appointments.ToList().ForEach(a => appointmentReader.AddKey(a.Id));
        await appointmentReader.ExecuteAsync();

        var doctorsReader = context.CreateBatchGet<DoctorDto>();
        appointmentReader.Results.ForEach(a => doctorsReader.AddKey(a.DoctorId));
        await doctorsReader.ExecuteAsync();

        return await DoctorDto.GetDoctorsAsync(context, doctorsReader.Results).ToListAsync();
    }

    public async Task StoreAsync(Doctor doctor)
    {
        await DoctorDto.SetDoctorAsync(context, doctor);
    }
}