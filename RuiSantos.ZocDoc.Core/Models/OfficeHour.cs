namespace RuiSantos.ZocDoc.Core.Models;

public class OfficeHour
{
    public DayOfWeek Week { get; set; }
    public IEnumerable<TimeSpan> Hours { get; set; }

    public OfficeHour()
    {
        Week = DayOfWeek.Monday;
        Hours = Enumerable.Empty<TimeSpan>();
    }

    public OfficeHour(DayOfWeek week, IEnumerable<TimeSpan> hours)
    {
        Week = week;
        Hours = hours;
    }
}
