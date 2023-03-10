using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using RuiSantos.ZocDoc.Api.Core;
using RuiSantos.ZocDoc.Core.Data;
using RuiSantos.ZocDoc.Core.Managers;
using RuiSantos.ZocDoc.Data.Mongodb;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddLogging(config => config.AddConsole());
builder.Services.AddMemoryCache();
builder.Services.AddControllers();

// Configure Autofac Dependency Injection container
var defaultConnectionString = builder.Configuration.GetConnectionString("Default");
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(container => 
{
    container.RegisterDataContext(defaultConnectionString);

    container.RegisterType<DomainContext>().As<IDomainContext>().SingleInstance();

    container.RegisterType<MedicalSpecialtiesManagement>().As<IMedicalSpecialtiesManagement>().InstancePerDependency();
    container.RegisterType<DoctorManagement>().As<IDoctorManagement>().InstancePerDependency();
    container.RegisterType<PatientManagement>().As<IPatientManagement>().InstancePerDependency();
    container.RegisterType<AppointmentManagement>().As<IAppointmentManagement>().InstancePerDependency();
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();