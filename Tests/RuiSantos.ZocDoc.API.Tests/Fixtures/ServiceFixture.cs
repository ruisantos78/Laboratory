using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Networks;
using RuiSantos.ZocDoc.API.Tests.Containers;

namespace RuiSantos.ZocDoc.API.Tests.Fixtures;

public class ServiceFixture : IAsyncLifetime
{
    private readonly DynamoDbContainer dynamoDbContainer;
    private readonly ZocDocAPIContainer zocDocAPIContainer;

    public HttpClient Client { get; private set; }

    private readonly INetwork network;

    public ServiceFixture()
    {
        this.dynamoDbContainer = new DynamoDbContainer();
        this.zocDocAPIContainer = new ZocDocAPIContainer();

        this.Client = new HttpClient();

        this.network = new NetworkBuilder()
          .WithName(Guid.NewGuid().ToString("D"))
          .Build();        
    }

    public async Task DisposeAsync()
    {
        await this.zocDocAPIContainer.DisposeAsync();
        await this.dynamoDbContainer.DisposeAsync();
        await this.network.DisposeAsync();
    }

    public async Task InitializeAsync()
    {
        await network.CreateAsync();

        await zocDocAPIContainer.BuildAsync();

        await dynamoDbContainer.StartAsync(network);
        await zocDocAPIContainer.StartAsync(dynamoDbContainer, network);

        this.Client.BaseAddress = zocDocAPIContainer.GetBaseAddress();
    }
}