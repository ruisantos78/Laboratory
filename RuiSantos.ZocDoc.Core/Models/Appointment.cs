namespace RuiSantos.ZocDoc.Core.Models;

public class Appointment
{
    public Guid Id { get; set; }
    public DayOfWeek Week { get; set; }
    public DateOnly Date { get; set; }
    public TimeSpan Time { get; set; }

    public Appointment()
    {
        Id = Guid.NewGuid();
        Week = DayOfWeek.Monday;
        Date = DateOnly.MinValue;
        Time = TimeSpan.MinValue;
    }

    public Appointment(DateTime date)
    {
        Id = Guid.NewGuid();
        Week = date.DayOfWeek;
        Date = DateOnly.FromDateTime(date);
        Time = date.TimeOfDay;
    }

    public DateTime GetDateTime() => Date.ToDateTime(Time);
}

