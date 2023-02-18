using MongoDB.Driver;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Data.Mongodb.Core;
using System.Linq.Expressions;

namespace RuiSantos.ZocDoc.Data.Mongodb.Entities;

internal class DoctorEntity : IEntity<Doctor>
{
    public const string Discriminator = "Doctors";

    public DoctorEntity() : base() { }

    public async Task StoreAsync(IMongoDatabase context, Doctor model)
    {
        var collection = context.GetCollection<Doctor>(Discriminator);

        await collection.FindOneAndDeleteAsync(entity => entity.Id == model.Id);
        await collection.InsertOneAsync(model);
    }

    public Task RemoveAsync(IMongoDatabase context, Guid id)
    {
        return context.GetCollection<Doctor>(Discriminator)
            .FindOneAndDeleteAsync(entity => entity.Id == id);
    }

    public Task<bool> AnyAsync(IMongoDatabase context, Expression<Func<Doctor, bool>> expression)
    {
        return context.GetCollection<Doctor>(Discriminator)
            .Find(expression)
            .AnyAsync();
    }

    public Task<List<Doctor>> ListAsync(IMongoDatabase context)
    {
        return context.GetCollection<Doctor>(Discriminator)
            .AsQueryable()
            .ToListAsync();
    }

    public Task<List<Doctor>> QueryAsync(IMongoDatabase context, Expression<Func<Doctor, bool>> expression)
    {
        return context.GetCollection<Doctor>(Discriminator)
            .Find(expression)
            .ToListAsync();
    }

    public Task<Doctor> FindAsync(IMongoDatabase context, Guid id)
    {
        return context.GetCollection<Doctor>(Discriminator)
            .Find(entity => entity.Id == id)
            .FirstOrDefaultAsync();
    }

    public Task<Doctor> FindAsync(IMongoDatabase context, Expression<Func<Doctor, bool>> expression)
    {
        return context.GetCollection<Doctor>(Discriminator)
            .Find(expression)
            .FirstOrDefaultAsync();
    }
}