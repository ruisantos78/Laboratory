using MongoDB.Driver;
using RuiSantos.ZocDoc.Core.Adapters;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Data.Mongodb.Mappings;

namespace RuiSantos.ZocDoc.Data.Mongodb.Adapters;

public class PatientAdapter : IPatientAdapter
{
    private readonly IMongoDatabase context;
    private readonly IMongoCollection<Patient> collection;

    public PatientAdapter(IMongoDatabase context)
    {
        this.context = context;
        this.collection = context.GetCollection<Patient>(PatientClassMap.Discriminator);
    }

    public Task<List<Patient>> FindAllWithAppointmentsAsync(IEnumerable<Appointment> appointments)
    {
        if (appointments?.Any() is not true)
            return Task.FromResult(new List<Patient>());

        return collection.Find(p => p.Appointments.Any(a => appointments.Any(ap => ap.Id == a.Id))).ToListAsync(); 
    }

    public async Task<Patient?> FindAsync(string socialSecurityNumber)
    {
        if (string.IsNullOrWhiteSpace(socialSecurityNumber))
            return null;

        return await this.collection.Find(p => p.SocialSecurityNumber == socialSecurityNumber).FirstOrDefaultAsync();
    }
        
    public async Task StoreAsync(Patient patient)
    {
        await collection.FindOneAndDeleteAsync(entity => entity.Id == patient.Id);
        await collection.InsertOneAsync(patient);
    }
}