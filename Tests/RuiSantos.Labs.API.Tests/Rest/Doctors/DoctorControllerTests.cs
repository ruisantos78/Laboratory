﻿using Amazon.DynamoDBv2.DataModel;
using RuiSantos.Labs.API.Tests.Fixtures;
using Xunit.Abstractions;

namespace RuiSantos.Labs.API.Tests.Rest;

[Collection(nameof(ServiceCollectionFixture))]
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