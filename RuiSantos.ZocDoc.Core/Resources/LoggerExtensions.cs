using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace RuiSantos.ZocDoc.Core;

internal static class LoggerExtensions
{
    public static void LogException(this ILogger logger, Exception ex)
    {
        var stackTrace = new StackTrace(ex, true);
        var frame = stackTrace.GetFrame(0);
        var method = frame!.GetMethod();
        var className = method!.DeclaringType!.FullName;
        var methodName = method.Name;

        logger?.LogError(ex, "Error on {Class}.{Method}: {Message}", className, methodName, ex.Message);
    }

}

