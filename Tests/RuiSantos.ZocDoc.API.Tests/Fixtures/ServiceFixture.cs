using Microsoft.AspNetCore.TestHost;
using RuiSantos.ZocDoc.API.Tests.Containers;

namespace RuiSantos.ZocDoc.API.Tests.Fixtures;

public class ServiceFixture : IAsyncLifetime
{
    private readonly DynamoDbContainer dynamoDbContainer;

    private ZocDocWebHost? zocDocWebHost = null;

    public TestServer Server => zocDocWebHost?.Server
        ?? throw ArgumentNullException();

    private Exception ArgumentNullException()
    {
        throw new NotImplementedException();
    }

    public ServiceFixture()
    {
        this.dynamoDbContainer = new DynamoDbContainer();
    }

    public async Task DisposeAsync()
    {
        this.zocDocWebHost?.Dispose();
        await this.dynamoDbContainer.DisposeAsync();
    }

    public async Task InitializeAsync()
    {
        await dynamoDbContainer.StartAsync();
        this.zocDocWebHost = new ZocDocWebHost(dynamoDbContainer.GetConnectionString());
    }
}