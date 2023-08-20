using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace RuiSantos.ZocDoc.Core;

/// <summary>
/// Logger extension methods.
/// </summary>
internal static class LoggerExtensions
{
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
