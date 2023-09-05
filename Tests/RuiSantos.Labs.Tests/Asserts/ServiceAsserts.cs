using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace RuiSantos.Labs.Tests.Asserts;

internal abstract class ServiceAsserts<TService>
{
    protected ILogger<TService> Logger { get; }

    public ServiceAsserts()
    {
        Logger = Substitute.For<ILogger<TService>>();
    }

    [SuppressMessage("Usage", "NS5000:Received check.", Justification = "Logger")]
    [SuppressMessage("Non-substitutable member", "NS1001:Non-virtual setup specification.", Justification = "<Pending>")]
    public void ShouldNotLogError()
    {
        Logger.DidNotReceiveWithAnyArgs().LogError(default);
    }

    [SuppressMessage("Usage", "NS5000:Received check.", Justification = "Logger")]
    [SuppressMessage("Non-substitutable member", "NS1001:Non-virtual setup specification.", Justification = "<Pending>")]
    public void ShouldLogError()
    {
        Logger.ReceivedWithAnyArgs().LogError(default);
    }
}