using MongoDB.Driver;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Data.Mongodb.Core.Interfaces;
using System.Linq.Expressions;

namespace RuiSantos.ZocDoc.Data.Mongodb.Entities;

internal sealed class PatientEntity: IEntity<Patient>
{
    public const string Discriminator = "Patients";

    public async Task StoreAsync(IMongoDatabase context, Patient model)
    {
        var collection = context.GetCollection<Patient>(Discriminator);
        
        await collection.FindOneAndDeleteAsync(entity => entity.Id == model.Id);        
        await collection.InsertOneAsync(model);
    }

    public Task RemoveAsync(IMongoDatabase context, Guid id)
    {
        return context.GetCollection<Patient>(Discriminator)
            .FindOneAndDeleteAsync(entity => entity.Id == id);
    }

    public Task<bool> AnyAsync(IMongoDatabase context, Expression<Func<Patient, bool>> expression)
    {
        return context.GetCollection<Patient>(Discriminator)
            .Find(expression)
            .AnyAsync();
    }

    public Task<List<Patient>> ListAsync(IMongoDatabase context)
    {
        return context.GetCollection<Patient>(Discriminator)
            .AsQueryable()
            .ToListAsync();
    }

    public Task<List<Patient>> QueryAsync(IMongoDatabase context, Expression<Func<Patient, bool>> expression)
    {
        return context.GetCollection<Patient>(Discriminator)
            .Find(expression)
            .ToListAsync();
    }

    public Task<Patient> FindAsync(IMongoDatabase context, Guid id)
    {
        return context.GetCollection<Patient>(Discriminator)
            .Find(entity => entity.Id == id)
            .FirstOrDefaultAsync();
    }

    public Task<Patient> FindAsync(IMongoDatabase context, Expression<Func<Patient, bool>> expression)
    {
        return context.GetCollection<Patient>(Discriminator)
            .Find(expression)
            .FirstOrDefaultAsync();
    }
}