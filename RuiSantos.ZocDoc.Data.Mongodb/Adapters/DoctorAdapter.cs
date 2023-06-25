using MongoDB.Driver;
using RuiSantos.ZocDoc.Core.Adapters;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Data.Mongodb.Mappings;

namespace RuiSantos.ZocDoc.Data.Mongodb.Adapters;

public class DoctorAdapter : IDoctorAdapter
{
    private readonly IMongoDatabase context;
    private readonly IMongoCollection<Doctor> collection;

    public DoctorAdapter(IMongoDatabase context)
    {
        this.context = context;
        this.collection = context.GetCollection<Doctor>(DoctorClassMap.Discriminator);
    }

    public Task<List<Doctor>> FindBySpecialtyWithAvailabilityAsync(string specialty, DateOnly date)
    {
        if (string.IsNullOrEmpty(specialty))
            return Task.FromResult(new List<Doctor>());

        return collection.Find(d => 
            d.Specialties.Contains(specialty)
            && d.OfficeHours.Count != d.Appointments.Count
            && d.OfficeHours.Any(oh => oh.Week == date.DayOfWeek))
            .ToListAsync();
    }
        
    public async Task<Doctor?> FindAsync(string license)
    {
        if (!string.IsNullOrEmpty(license))
            return await this.collection.Find(x => x.License == license).FirstOrDefaultAsync();

        return null;
    }       

    public async Task StoreAsync(Doctor doctor)
    {
        await collection.FindOneAndDeleteAsync(x => x.License == doctor.License);
        await collection.InsertOneAsync(doctor);
    }

    public Task<List<Doctor>> FindBySpecialityAsync(string specialty)
    {
        if (!string.IsNullOrEmpty(specialty))
            return collection.Find(x => x.Specialties.Contains(specialty)).ToListAsync();

        return Task.FromResult(new List<Doctor>());
    }

    public Task<List<Doctor>> FindAllWithAppointmentsAsync(HashSet<Appointment> appointments)
    {
        if (!appointments.Any())
            return Task.FromResult(new List<Doctor>());

        return collection.Find(d => d.Appointments.Any(da => appointments.Any(pa => da.Id == pa.Id)))
            .ToListAsync();
    }
}
