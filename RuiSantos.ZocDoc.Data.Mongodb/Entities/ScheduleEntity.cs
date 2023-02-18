using MongoDB.Driver;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Data.Mongodb.Core;

namespace RuiSantos.ZocDoc.Data.Mongodb.Entities;

internal class ScheduleEntity: Schedule, IEntity<Schedule>
{
    public const string Discriminator = "Schedules";

    public string? DoctorReference => this.Doctor.Id;
    public IReadOnlyList<string?> AppointmentsReferences => this.Appointments.Select(a => a.Id).ToArray();

    public ScheduleEntity(): base() { }

    public Task StoreAsync(IMongoDatabase context, Schedule model) => this.StoreAsync(Discriminator, model, context, GetFilter(model.Id));

    public Task RemoveAsync(IMongoDatabase context, string id)
    {
        return context.GetCollection<ScheduleEntity>(Discriminator)
            .FindOneAndDeleteAsync(GetFilter(id));
    }

    public IQueryable<Schedule> Query(IMongoDatabase context)
    {
        var schedules = context.GetCollection<ScheduleEntity>(ScheduleEntity.Discriminator).AsQueryable();
        var doctors = context.GetCollection<DoctorEntity>(DoctorEntity.Discriminator).AsQueryable();
        var appointments = context.GetCollection<AppointmentEntity>(AppointmentEntity.Discriminator).AsQueryable();

        return from schedule in schedules
               join doctor in doctors on schedule.DoctorReference equals doctor.Id
               select new Schedule
               {
                   Id = schedule.Id,
                   Availability = schedule.Availability,
                   Doctor = doctor,
                   Appointments = appointments.Where(a => schedule.AppointmentsReferences.Contains(a.Id)).Cast<Appointment>().ToList()
               };
    }

    public Schedule Find(IMongoDatabase context, string id)
    {
        return context.GetCollection<ScheduleEntity>(Discriminator)
            .Find(GetFilter(id))
            .FirstOrDefault();
    }

    private static FilterDefinition<ScheduleEntity> GetFilter(string? id) => Builders<ScheduleEntity>.Filter.Eq(e => e.Id, id);
}