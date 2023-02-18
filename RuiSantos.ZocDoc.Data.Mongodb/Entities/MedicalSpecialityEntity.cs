using System;
using MongoDB.Driver;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Data.Mongodb.Core;

namespace RuiSantos.ZocDoc.Data.Mongodb.Entities;

internal class MedicalSpecialityEntity: MedicalSpeciality, IEntity<MedicalSpeciality>
{
    public const string Discriminator = "MedicalSpecialities";

    public MedicalSpecialityEntity() : base() { }

    public Task StoreAsync(IMongoDatabase context, MedicalSpeciality model) => this.StoreAsync(Discriminator, model, context, GetFilter(model.Id));

    public Task RemoveAsync(IMongoDatabase context, string id)
    {
        return context.GetCollection<MedicalSpecialityEntity>(Discriminator)
            .FindOneAndDeleteAsync(GetFilter(id));    
    }

    public IQueryable<MedicalSpeciality> Query(IMongoDatabase context)
    {
        return context.GetCollection<MedicalSpecialityEntity>(Discriminator)
            .AsQueryable();
    }

    public MedicalSpeciality Find(IMongoDatabase context, string id)
    {
        return context.GetCollection<MedicalSpecialityEntity>(Discriminator)
            .Find(GetFilter(id))
            .FirstOrDefault();            
    }
    
    private static FilterDefinition<MedicalSpecialityEntity> GetFilter(string? id) => Builders<MedicalSpecialityEntity>.Filter.Eq(e => e.Description, id);
}