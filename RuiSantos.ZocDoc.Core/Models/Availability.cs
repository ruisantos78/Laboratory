using System;
namespace RuiSantos.ZocDoc.Core.Models;

public class Availability
{
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public int Duration { get; set; }

    public Availability()
    {
        this.DayOfWeek = DayOfWeek.Monday;
        this.StartTime = TimeOnly.MinValue;
        this.Duration = 0;
    }
}