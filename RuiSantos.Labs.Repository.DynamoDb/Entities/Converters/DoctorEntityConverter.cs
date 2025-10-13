using RuiSantos.Labs.Domain.Extensions.Models;

namespace RuiSantos.Labs.Repository.DynamoDb.Entities.Converters;

internal static class DoctorEntityConverter
{
    public static Doctor ToDoctor(this DoctorEntity entity, IEnumerable<string> specializations)
    {
        return new Doctor
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Email = entity.Email,
            ContactNumbers = [..entity.ContactNumbers],
            OfficeHours = [..entity.Availability.Select(ToOfficeHour)],
            Specializations = [..specializations]
        };
    }
    
    private static OfficeHour ToOfficeHour(this AvailabilityEntity model)
    {
        return new OfficeHour
        {
            Week = model.DayOfWeek,
            Hours = model.Hours,
        };
    }
    
    public static DoctorEntity ToDoctorEntity(this Doctor model)
    {
        return new DoctorEntity
        {
            Id = model.Id,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            ContactNumbers = [..model.ContactNumbers],
            Availability = [..model.OfficeHours.Select(ToAvailabilityEntity)]
        };
    }
    
    private static AvailabilityEntity ToAvailabilityEntity(this OfficeHour model)
    {
        return new AvailabilityEntity
        {
            DayOfWeek = model.Week,
            Hours = [..model.Hours],
        };
    }
}