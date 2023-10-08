using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace RuiSantos.Labs.Tests.Asserts;

internal abstract class LoggerAsserts<TAssert>
{
    protected readonly ILogger<TAssert> Logger = Substitute.For<ILogger<TAssert>>();

    [SuppressMessage("Usage", "NS5000:Received check.", Justification = "Logger")]
    [SuppressMessage("Non-substitutable member", "NS1001:Non-virtual setup specification.", Justification = "<Pending>")]
    public void ShouldLogError(bool received = true)
    {
        if (received)
            Logger.ReceivedWithAnyArgs().LogError(default);
        else
            Logger.DidNotReceiveWithAnyArgs().LogError(default);
    }
}