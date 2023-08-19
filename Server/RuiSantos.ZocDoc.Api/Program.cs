using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using RuiSantos.ZocDoc.Api.Core;
using RuiSantos.ZocDoc.Core;
using RuiSantos.ZocDoc.Data;
using RuiSantos.ZocDoc.GraphQL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddLogging(config => config.AddConsole());
builder.Services.AddMemoryCache();
builder.Services.AddControllers();
builder.Services.AddZocDocGraphQL();

// Configure Autofac Dependency Injection container
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(container => 
{
    container.RegisterZocDocServices();
    container.UseMongoDb();
    // container.UseDynamoDb(); 
});

builder.Services.AddSwaggerGen(options =>
{
    options.OperationFilter<ReApplyOptionalRouteParameterOperationFilter>();

    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "ZocDoc from Rui Santos",
        Description = "A simple ZocDoc implementation by Rui Santos"
    });    

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();
app.MapGraphQL();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(x => x.SwaggerEndpoint("/swagger/v1/swagger.yaml", "ZocDoc from Rui Santos v1")); ;
}

app.Run();