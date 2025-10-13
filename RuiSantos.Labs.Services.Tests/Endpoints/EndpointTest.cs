using Amazon.DynamoDBv2.DataModel;
using RuiSantos.Labs.Repository.DynamoDb.Entities;
using RuiSantos.Labs.Services.Tests.Fixtures;

namespace RuiSantos.Labs.Services.Tests.Endpoints;

public abstract class EndpointTest(ServiceFixture fixture) : IAsyncLifetime
{
    protected DynamoDBContext Context { get; } = fixture.CreateDynamoDbContext();
    protected HttpClient Client { get; } = fixture.CreateHttpClient();

    public async Task InitializeAsync()
    {
        await CleanupAsync();
    }

    public async Task DisposeAsync()
    {
        await CleanupAsync();
        
        Context.Dispose();
        Client.Dispose();
    }

    protected virtual async Task CleanupAsync()
    {
        await Task.WhenAll(
            Context.DeleteAllAsync<DoctorEntity>(),
            Context.DeleteAllAsync<DoctorSpecializationEntity>()
        );
    }
}