using MongoDB.Driver;
using RuiSantos.ZocDoc.Core.Adapters;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Data.Mongodb.Mappings;

namespace RuiSantos.ZocDoc.Data.Mongodb.Adapters;

public class MedicalSpecialityAdapter : IMedicalSpecialityAdapter
{
    private readonly IMongoDatabase context;
    private readonly IMongoCollection<MedicalSpecialty> collection;

    public MedicalSpecialityAdapter(IMongoDatabase context)
    {
        this.context = context;
        this.collection = context.GetCollection<MedicalSpecialty>(MedicalSpecialityClassMap.Discriminator);
    }

    public Task AddAsync(MedicalSpecialty speciality)
    {
        if (speciality is not null)
            return collection.InsertOneAsync(speciality);

        return Task.CompletedTask;
    }

    public Task<bool> ContainsAsync(string speciality)
    {
        return collection.Find(x => x.Description.Equals(speciality, StringComparison.OrdinalIgnoreCase)).AnyAsync();
    }

    public Task RemoveAsync(string speciality)
    {
        if (!string.IsNullOrWhiteSpace(speciality))
            return collection.DeleteOneAsync(x => x.Description.Equals(speciality, StringComparison.OrdinalIgnoreCase));

        return Task.CompletedTask;
    }

    public Task<List<MedicalSpecialty>> ToListAsync()
    {
        return collection.Find(_ => true).ToListAsync();
    }
}
