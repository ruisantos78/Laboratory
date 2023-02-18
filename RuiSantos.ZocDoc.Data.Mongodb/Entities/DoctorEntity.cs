using MongoDB.Driver;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Data.Mongodb.Core;

namespace RuiSantos.ZocDoc.Data.Mongodb.Entities;

internal class DoctorEntity: Doctor, IEntity<Doctor>
{
    public const string Discriminator = "Doctors";

    public DoctorEntity(): base() {  }

    public Task StoreAsync(IMongoDatabase context, Doctor model) => this.StoreAsync(Discriminator, model, context, GetFilter(model.Id));

    public Task RemoveAsync(IMongoDatabase context, string id)
    {
        return context.GetCollection<DoctorEntity>(Discriminator)
            .FindOneAndDeleteAsync(GetFilter(id));
    }

    public IQueryable<Doctor> Query(IMongoDatabase context)
    {
        return context.GetCollection<DoctorEntity>(DoctorEntity.Discriminator)
            .AsQueryable();
    }

    public Doctor Find(IMongoDatabase context, string id)
    {
        return context.GetCollection<DoctorEntity>(Discriminator)
            .Find(GetFilter(id))
            .FirstOrDefault();
    }

    private static FilterDefinition<DoctorEntity> GetFilter(string? id) => Builders<DoctorEntity>.Filter.Eq(e => e.License, id);
}