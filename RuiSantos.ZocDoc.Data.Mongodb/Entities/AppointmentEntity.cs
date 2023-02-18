using MongoDB.Driver;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Data.Mongodb.Core;
using System.Linq.Expressions;

namespace RuiSantos.ZocDoc.Data.Mongodb.Entities;

internal class AppointmentEntity : IEntity<Appointment>
{
    public const string Discriminator = "Appointments";

    public async Task StoreAsync(IMongoDatabase context, Appointment model)
    {
        var collection = context.GetCollection<Appointment>(Discriminator);

        await collection.FindOneAndDeleteAsync(entity => entity.Id == model.Id);
        await collection.InsertOneAsync(model);
    }

    public Task RemoveAsync(IMongoDatabase context, Guid id)
    {
        return context.GetCollection<Appointment>(Discriminator)
            .FindOneAndDeleteAsync(entity => entity.Id == id);
    }

    public Task<bool> AnyAsync(IMongoDatabase context, Expression<Func<Appointment, bool>> expression)
    {
        return context.GetCollection<Appointment>(Discriminator)
            .Find(expression)
            .AnyAsync();
    }

    public Task<List<Appointment>> ListAsync(IMongoDatabase context)
    {
        return context.GetCollection<Appointment>(Discriminator)
            .AsQueryable()
            .ToListAsync();
    }

    public Task<List<Appointment>> QueryAsync(IMongoDatabase context, Expression<Func<Appointment, bool>> expression)
    {
        return context.GetCollection<Appointment>(Discriminator)
            .Find(expression)
            .ToListAsync();
    }

    public Task<Appointment> FindAsync(IMongoDatabase context, Guid id)
    {
        return context.GetCollection<Appointment>(Discriminator)
            .Find(entity => entity.Id == id)
            .FirstOrDefaultAsync();
    }

    public Task<Appointment> FindAsync(IMongoDatabase context, Expression<Func<Appointment, bool>> expression)
    {
        return context.GetCollection<Appointment>(Discriminator)
            .Find(expression)
            .FirstOrDefaultAsync();
    }
}