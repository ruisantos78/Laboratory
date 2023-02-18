using System;
namespace RuiSantos.ZocDoc.Core.Models;

public class Appointment
{
    public Guid Id { get; set; }
    public DayOfWeek Week { get; set; }
    public DateOnly Date { get; set; }
    public TimeSpan Time { get; set; }

    public Appointment()
    {
        this.Id = Guid.NewGuid();
        this.Week = DayOfWeek.Monday;
        this.Date = DateOnly.MinValue;
        this.Time = TimeSpan.MinValue;
    }
}

