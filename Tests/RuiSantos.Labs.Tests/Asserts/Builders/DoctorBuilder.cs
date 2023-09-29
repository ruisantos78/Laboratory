using RuiSantos.Labs.Core.Models;

namespace RuiSantos.Labs.Tests.Asserts.Builders;

internal class DoctorBuilder
{
    private readonly Doctor model;

    public DoctorBuilder(Guid? doctorId = null)
    {
        model = new()
        {
            Id = doctorId ?? Guid.NewGuid()
        };
    }

    public Doctor Build()
    {
        return model;
    }

    public DoctorBuilder With(string? license = null, string? email = null, string? firstName = null, string? lastName = null)
    {
        model.License = license ?? model.License;
        model.Email = email ?? model.Email;
        model.FirstName = firstName ?? model.FirstName;
        model.LastName = lastName ?? model.LastName;
        
        return this;
    }

    public DoctorBuilder WithOfficeHours(DayOfWeek week, params string[] hours)
    {
        var timespans = hours.Select(x => TimeSpan.Parse(x));

        model.OfficeHours.RemoveWhere(x => x.Week == week);
        model.OfficeHours.Add(new OfficeHour(week, timespans));

        return this;
    }
}