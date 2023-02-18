namespace RuiSantos.ZocDoc.Core.Models;

public class OfficeHour
{
    public DayOfWeek Week { get; set; }
    public List<TimeSpan> Hours { get; set; }

    public OfficeHour()
    {
        this.Week= DayOfWeek.Monday;
        this.Hours = new List<TimeSpan>();
    }
}
