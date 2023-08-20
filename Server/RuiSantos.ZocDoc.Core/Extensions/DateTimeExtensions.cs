using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace RuiSantos.ZocDoc.Core;

/// <summary>
/// Internal extension methods.
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// Returns a DateTime with the same date as the DateOnly, but the same time as the TimeSpan.
    /// </summary>
    /// <param name="date">The DateOnly.</param>
    /// <param name="timeSpan">The TimeSpan.</param>
    /// <returns>A DateTime with the same date as the DateOnly, but the same time as the TimeSpan.</returns>    
    public static DateTime WithTime(this DateOnly date, TimeSpan timeSpan)
    {
        return date.ToDateTime(TimeOnly.FromTimeSpan(timeSpan));
    }
}
