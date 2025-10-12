using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace RuiSantos.Labs.Tests.Asserts.Providers;

/// <summary>
/// NSubstitute-based logger provider for testing
/// </summary>
public class SubstituteLoggerProvider(ILogger? logger) : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName)
    {
        return logger ?? Substitute.For<ILogger>();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// Extension methods for adding NSubstitute logging
/// </summary>
public static class SubstituteLoggingExtensions
{
    /// <summary>
    /// Adds NSubstitute mock loggers to the logging builder
    /// </summary>
    public static ILoggingBuilder AddSubstitute(this ILoggingBuilder builder, ILogger? logger = null)
    {
        builder.Services.AddSingleton<ILoggerProvider>(_ => new SubstituteLoggerProvider(logger));
        return builder;
    }
}
