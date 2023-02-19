using MongoDB.Driver;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Data.Mongodb.Core.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace RuiSantos.ZocDoc.Data.Mongodb.Entities;

internal sealed class MedicalSpecialityEntity : IEntity<MedicalSpeciality>
{
    public const string Discriminator = "MedicalSpecialities";

    public async Task StoreAsync(IMongoDatabase context, MedicalSpeciality model)
    {
        var collection = context.GetCollection<MedicalSpeciality>(Discriminator);

        await collection.FindOneAndDeleteAsync(entity => entity.Id == model.Id);
        await collection.InsertOneAsync(model);
    }

    public Task RemoveAsync(IMongoDatabase context, Guid id)
    {
        return context.GetCollection<MedicalSpeciality>(Discriminator)
            .FindOneAndDeleteAsync(entity => entity.Id == id);
    }

    public Task<bool> AnyAsync(IMongoDatabase context, Expression<Func<MedicalSpeciality, bool>> expression)
    {
        return context.GetCollection<MedicalSpeciality>(Discriminator)
            .Find(expression)
            .AnyAsync();
    }

    public Task<List<MedicalSpeciality>> ListAsync(IMongoDatabase context)
    {
        return context.GetCollection<MedicalSpeciality>(Discriminator)
            .AsQueryable()
            .ToListAsync();
    }
    public Task<List<MedicalSpeciality>> QueryAsync(IMongoDatabase context, Expression<Func<MedicalSpeciality, bool>> expression)
    {
        return context.GetCollection<MedicalSpeciality>(Discriminator)
            .Find(expression)
            .ToListAsync();
    }

    public Task<MedicalSpeciality> FindAsync(IMongoDatabase context, Guid id)
    {
        return context.GetCollection<MedicalSpeciality>(Discriminator)
            .Find(entity => entity.Id == id)
            .FirstOrDefaultAsync();
    }

    public Task<MedicalSpeciality> FindAsync(IMongoDatabase context, Expression<Func<MedicalSpeciality, bool>> expression)
    {
        return context.GetCollection<MedicalSpeciality>(Discriminator)
            .Find(expression)
            .FirstOrDefaultAsync();
    }
}