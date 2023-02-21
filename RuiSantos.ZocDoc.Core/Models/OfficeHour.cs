namespace RuiSantos.ZocDoc.Core.Models;

public class OfficeHour
{
    public DayOfWeek Week { get; init; }
    public IEnumerable<TimeSpan> Hours { get; init; }

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
