using MongoDB.Bson;
using MongoDB.Driver;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Data.Mongodb.Core;

namespace RuiSantos.ZocDoc.Data.Mongodb.Entities;

internal class AppointmentEntity: Appointment, IEntity<Appointment>
{
    public const string Discriminator = "Appointments";

    public string? PatientReference => this.Patient.Id;

    public AppointmentEntity() : base() { }

    public Task StoreAsync(IMongoDatabase context, Appointment model) => this.StoreAsync(Discriminator, model, context, GetFilter(model.Id));

    public Task RemoveAsync(IMongoDatabase context, string id)
    {
        return context.GetCollection<AppointmentEntity>(Discriminator)
            .FindOneAndDeleteAsync(GetFilter(id));
    }

    public IQueryable<Appointment> Query(IMongoDatabase context)
    {
        var appointments = context.GetCollection<AppointmentEntity>(AppointmentEntity.Discriminator).AsQueryable();
        var patients = context.GetCollection<PatientEntity>(PatientEntity.Discriminator).AsQueryable();

        return from appointment in appointments
               join patient in patients on appointment.PatientReference equals patient.Id
               select new Appointment
               {
                   Id = appointment.Id,
                   Date = appointment.Date,
                   Patient = patient
               };
    }

    public Appointment Find(IMongoDatabase context, string id)
    {
        return context.GetCollection<AppointmentEntity>(Discriminator)
            .Find(GetFilter(id))
            .FirstOrDefault();
    }

    private static FilterDefinition<AppointmentEntity> GetFilter(string? id) => Builders<AppointmentEntity>.Filter.Eq(e => e.Id, id);
}

