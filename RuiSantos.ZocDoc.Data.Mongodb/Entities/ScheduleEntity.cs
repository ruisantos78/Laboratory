using MongoDB.Driver;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Data.Mongodb.Core;
using System.Linq.Expressions;

namespace RuiSantos.ZocDoc.Data.Mongodb.Entities;

internal class ScheduleEntity : IEntity<Schedule>
{
    public const string Discriminator = "Schedules";
    public async Task StoreAsync(IMongoDatabase context, Schedule model)
    {
        var collection = context.GetCollection<Schedule>(Discriminator);
        
        await collection.FindOneAndDeleteAsync(entity => entity.Id == model.Id);        
        await collection.InsertOneAsync(model);
    }

    public Task RemoveAsync(IMongoDatabase context, Guid id)
    {
        return context.GetCollection<Schedule>(Discriminator)
            .FindOneAndDeleteAsync(entity => entity.Id == id);
    }

    public Task<bool> AnyAsync(IMongoDatabase context, Expression<Func<Schedule, bool>> expression)
    {
        return context.GetCollection<Schedule>(Discriminator)
            .Find(expression)
            .AnyAsync();
    }

    public Task<List<Schedule>> ListAsync(IMongoDatabase context)
    {
        return context.GetCollection<Schedule>(Discriminator)
            .AsQueryable()
            .ToListAsync();
    }

    public Task<List<Schedule>> QueryAsync(IMongoDatabase context, Expression<Func<Schedule, bool>> expression)
    {
        return context.GetCollection<Schedule>(Discriminator)
            .Find(expression)
            .ToListAsync();
    }

    public Task<Schedule> FindAsync(IMongoDatabase context, Guid id)
    {
        return context.GetCollection<Schedule>(Discriminator)
            .Find(entity => entity.Id == id)
            .FirstOrDefaultAsync();
    }

    public Task<Schedule> FindAsync(IMongoDatabase context, Expression<Func<Schedule, bool>> expression)
    {
        return context.GetCollection<Schedule>(Discriminator)
            .Find(expression)
            .FirstOrDefaultAsync();
    }
}