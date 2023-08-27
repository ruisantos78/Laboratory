﻿using Amazon.DynamoDBv2.DataModel;
using RuiSantos.ZocDoc.API.Tests.Fixtures;
using Xunit.Abstractions;

namespace RuiSantos.ZocDoc.API.Tests.Rest;

public partial class DoctorControllerTests: IClassFixture<ServiceFixture>
{
    private readonly IDynamoDBContext context;
    private readonly HttpClient client;
    private readonly ITestOutputHelper output;

    public DoctorControllerTests(ServiceFixture service, ITestOutputHelper output)
    {
        this.context = service.GetContext();
        this.client = service.GetClient();
        this.output = output;
    }
}