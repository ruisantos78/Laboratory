using Amazon.DynamoDBv2.DataModel;
using RuiSantos.Labs.Api.Tests.Fixtures;
using RuiSantos.Labs.GraphQL.Services;
using Xunit.Abstractions;

namespace RuiSantos.Labs.Api.Tests.Rest.Doctors;

[Collection(nameof(ServiceCollectionFixture))]
public partial class DoctorControllerTests: IClassFixture<ServiceFixture>
{
    protected readonly IDynamoDBContext Context;
    protected readonly HttpClient Client;
    protected readonly ITestOutputHelper Output;

    private readonly ISecurity _security = new Security();

    public DoctorControllerTests(ServiceFixture service, ITestOutputHelper output)
    {
        Context = service.GetContext();
        Client = service.GetClient();
        Output = output;
    }
}