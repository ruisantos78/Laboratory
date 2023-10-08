using Microsoft.Extensions.Logging;
using NSubstitute;

namespace RuiSantos.Labs.Tests.Asserts;

internal abstract class LoggerAsserts<TAssert>
{
    protected readonly ILogger<TAssert> Logger = Substitute.For<ILogger<TAssert>>();

    public void ShouldLogError(bool received = true)
    {
        if (received)
            Logger.Received().Log(LogLevel.Error,
                Arg.Any<EventId>(),
                Arg.Any<object>(),
                Arg.Any<Exception>(),
                Arg.Any<Func<object, Exception?, string>>());
        else
            Logger.DidNotReceive().Log(LogLevel.Error,
                Arg.Any<EventId>(),
                Arg.Any<object>(),
                Arg.Any<Exception>(),
                Arg.Any<Func<object, Exception?, string>>());
    }
}