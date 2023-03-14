using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace RuiSantos.ZocDoc.Core;

/// <summary>
/// Internal extension methods.
/// </summary>
internal static class InternalExtensions
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

    /// <summary>
    /// Writes the exception to the logger.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="ex">The exception.</param>
    public static void Fail(this ILogger logger, Exception ex)
    {
        var stackTrace = new StackTrace(ex, true);
        var frame = stackTrace.GetFrame(0);
        var method = frame!.GetMethod();
        var className = method!.DeclaringType!.FullName;
        var methodName = method.Name;

        logger?.LogError(ex, "Error on {Class}.{Method}: {Message}", className, methodName, ex.Message);
    }
}
