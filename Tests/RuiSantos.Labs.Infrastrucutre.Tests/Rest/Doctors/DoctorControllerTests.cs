using Amazon.DynamoDBv2.DataModel;
using RuiSantos.Labs.Infrastrucutre.Tests.Fixtures;
using Xunit.Abstractions;

namespace RuiSantos.Labs.Infrastrucutre.Tests.Rest.Doctors;

public partial class DoctorControllerTests: IClassFixture<ServiceFixture>
{
    protected readonly IDynamoDBContext Context;
    protected readonly HttpClient Client;
    protected readonly ITestOutputHelper Output;

    public DoctorControllerTests(ServiceFixture service, ITestOutputHelper output)
    {
        Context = service.GetContext();
        Client = service.GetClient();
        Output = output;
    }
}