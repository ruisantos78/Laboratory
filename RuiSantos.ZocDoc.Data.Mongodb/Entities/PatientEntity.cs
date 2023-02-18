using MongoDB.Driver;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Data.Mongodb.Core;

namespace RuiSantos.ZocDoc.Data.Mongodb.Entities;

public class PatientEntity: Patient, IEntity<Patient>
{
    public const string Discriminator = "Patients";

    public PatientEntity(): base() { }

    public Task StoreAsync(IMongoDatabase context, Patient model) => this.StoreAsync(Discriminator, model, context, GetFilter(model.Id));

    public Task RemoveAsync(IMongoDatabase context, string id)
    {
        return context.GetCollection<PatientEntity>(Discriminator)
            .FindOneAndDeleteAsync(GetFilter(id));
    }

    public IQueryable<Patient> Query(IMongoDatabase context)
    {
        return context.GetCollection<PatientEntity>(Discriminator)
            .AsQueryable();
    }

    public Patient Find(IMongoDatabase context, string id)
    {
        return context.GetCollection<PatientEntity>(Discriminator)
            .Find(GetFilter(id))
            .FirstOrDefault();
    }

    private static FilterDefinition<PatientEntity> GetFilter(string? id) => Builders<PatientEntity>.Filter.Eq(e => e.SocialSecurityNumber, id);
}