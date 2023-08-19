using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RuiSantos.ZocDoc.Core.Models;

/// <summary>
/// Represents an office hour.
/// </summary>
public class OfficeHour
{
    /// <summary>
    /// The day of the week.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public DayOfWeek Week { get; init; }

    /// <summary>
    /// Array of working hours.
    /// </summary>
    [JsonConverter(typeof(TimeSpanConverter))]
    public IEnumerable<TimeSpan> Hours { get; init; }

    /// <summary>
    /// Creates a new instance of OfficeHour.
    /// </summary>
    public OfficeHour()
    {
        Week = DayOfWeek.Monday;
        Hours = Enumerable.Empty<TimeSpan>();
    }

    /// <summary>
    /// Creates a new instance of OfficeHour.
    /// </summary>
    /// <param name="week">The day of the week</param>
    /// <param name="hours">The array of working hours.</param>
    public OfficeHour(DayOfWeek week, IEnumerable<TimeSpan> hours)
    {
        Week = week;
        Hours = hours;
    }
}
