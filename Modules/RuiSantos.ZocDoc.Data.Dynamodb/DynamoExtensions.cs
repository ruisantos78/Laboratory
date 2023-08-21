using Amazon.DynamoDBv2;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using RuiSantos.ZocDoc.Core.Repositories;
using RuiSantos.ZocDoc.Data.Dynamodb.Repositories;
using RuiSantos.ZocDoc.Data.Dynamodb.Mediators;
using Amazon.Runtime;

namespace RuiSantos.ZocDoc.Data;

public static class DynamoExtensions
{
    private static string ConnectionString => Environment.GetEnvironmentVariable("DATABASE_DYNAMO")
        ?? "http://127.0.0.1:8000";        

    public static IServiceCollection AddZocDocDynamoDb(this IServiceCollection services)
    {
        services.AddSingleton<IAmazonDynamoDB>(provider =>
        {
            var credentials = new BasicAWSCredentials("api", "secret");
            var config = new AmazonDynamoDBConfig() { ServiceURL = ConnectionString };
            var client = new AmazonDynamoDBClient(credentials, config);
            RegisterClassMaps.InitializeDatabase(client);

            return client;
        });

        services.AddTransient<IMedicalSpecialityRepository, MedicalSpecialityRepository>();
        services.AddTransient<IPatientRepository, PatientRepository>();
        services.AddTransient<IDoctorRepository, DoctorRepository>();
        services.AddTransient<IAppointamentsRepository, AppointamentsRepository>();

        return services;
    }

    public static ContainerBuilder UseZocDocDynamoDb(this ContainerBuilder builder)
    {
        builder.Register(provider =>
        {
            var config = new AmazonDynamoDBConfig() { ServiceURL = ConnectionString };
            var client = new AmazonDynamoDBClient(config);
            RegisterClassMaps.InitializeDatabase(client);

            return client;
        }).As<IAmazonDynamoDB>().SingleInstance();

        builder.RegisterType<MedicalSpecialityRepository>().As<IMedicalSpecialityRepository>();
        builder.RegisterType<PatientRepository>().As<IPatientRepository>();
        builder.RegisterType<DoctorRepository>().As<IDoctorRepository>();
        builder.RegisterType<AppointamentsRepository>().As<IAppointamentsRepository>();

        return builder;
    }
}
