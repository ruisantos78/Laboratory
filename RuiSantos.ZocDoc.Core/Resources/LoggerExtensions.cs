using System;
using Microsoft.Extensions.Logging;
using RuiSantos.ZocDoc.Core.Managers;

namespace RuiSantos.ZocDoc.Core.Resources;

internal static class LoggerExtensions
{
    public static void LogException(this ILogger logger, string className, string methodName, Exception ex)
    {
        logger?.LogError(ex, "Error on {Class}.{Method}: {Message}", className, methodName, ex.Message);
    }

}

