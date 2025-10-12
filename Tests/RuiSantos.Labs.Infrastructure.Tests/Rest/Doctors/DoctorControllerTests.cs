using Amazon.DynamoDBv2.DataModel;
using RuiSantos.Labs.Infrastructure.Tests.Fixtures;

namespace RuiSantos.Labs.Infrastructure.Tests.Rest.Doctors;

public partial class DoctorControllerTests : IClassFixture<ServiceFixture>
{
    protected readonly IDynamoDBContext Context;
    protected readonly HttpClient Client;
    protected readonly ITestOutputHelper Output;

    public DoctorControllerTests(ServiceFixture service, ITestOutputHelper output)
    {
        Context = service.GetDynamoDbContext();
        Client = service.GetHttpClient();
        Output = output;
    }
}