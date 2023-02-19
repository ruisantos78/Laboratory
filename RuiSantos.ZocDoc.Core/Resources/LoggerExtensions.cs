using Microsoft.Extensions.Logging;

namespace RuiSantos.ZocDoc.Core;

internal static class LoggerExtensions
{
    public static void LogException(this ILogger logger, string className, string methodName, Exception ex)
    {
        logger?.LogError(ex, "Error on {Class}.{Method}: {Message}", className, methodName, ex.Message);
    }
}

