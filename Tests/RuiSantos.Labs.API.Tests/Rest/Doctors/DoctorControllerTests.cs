﻿using Amazon.DynamoDBv2.DataModel;
using RuiSantos.Labs.Api.Tests.Fixtures;
using RuiSantos.Labs.GraphQL.Services;
using Xunit.Abstractions;

namespace RuiSantos.Labs.Api.Tests.Rest;

[Collection(nameof(ServiceCollectionFixture))]
public partial class DoctorControllerTests: IClassFixture<ServiceFixture>
{
    private readonly IDynamoDBContext context;
    private readonly HttpClient client;
    private readonly ITestOutputHelper output;

    private readonly ISecurity security = new Security();

    public DoctorControllerTests(ServiceFixture service, ITestOutputHelper output)
    {
        this.context = service.GetContext();
        this.client = service.GetClient();
        this.output = output;
    }
}