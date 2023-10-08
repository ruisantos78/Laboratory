using RuiSantos.Labs.Core.Models;

namespace RuiSantos.Labs.Tests.Asserts.Builders;

internal sealed class DoctorBuilder
{
    private readonly Doctor _model;

    public DoctorBuilder(Guid? doctorId = null)
    {
        _model = new()
        {
            Id = doctorId ?? Guid.NewGuid()
        };
    }

    public Doctor Build() => _model;

    public DoctorBuilder With(string? license = null, string? email = null, string? firstName = null,
        string? lastName = null)
    {
        _model.License = license ?? _model.License;
        _model.Email = email ?? _model.Email;
        _model.FirstName = firstName ?? _model.FirstName;
        _model.LastName = lastName ?? _model.LastName;

        return this;
    }

    public DoctorBuilder WithOfficeHours(DayOfWeek week, params string[] hours)
    {
        var timespans = hours.Select(TimeSpan.Parse);

        _model.OfficeHours.RemoveWhere(x => x.Week == week);
        _model.OfficeHours.Add(new OfficeHour(week, timespans));

        return this;
    }
}