using Amazon.DynamoDBv2;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using RuiSantos.ZocDoc.Core.Adapters;
using RuiSantos.ZocDoc.Data.Dynamodb.Adapters;
using RuiSantos.ZocDoc.Data.Dynamodb.Mappings;

namespace RuiSantos.ZocDoc.Data;

public static class DynamoExtensions
{
    private static readonly Lazy<AmazonDynamoDBClient> Database = new(() => {
        var connectionString = Environment.GetEnvironmentVariable("DATABASE_MONGO");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Please set the 'DATABASE_MONGO' before start the application.");

        var config = new AmazonDynamoDBConfig
        {
            ServiceURL = connectionString
        };
        return new AmazonDynamoDBClient(config);            
    });

    public static IServiceCollection AddDataContext(this IServiceCollection services)
    {
        RegisterClassMaps.InitializeDatabase(Database.Value);

        services.AddTransient<IMedicalSpecialityAdapter>(x => new MedicalSpecialityAdapter(Database.Value));
        services.AddTransient<IPatientAdapter>(x => new PatientAdapter(Database.Value));
        services.AddTransient<IDoctorAdapter>(x => new DoctorAdapter(Database.Value));

        return services;
    }

    public static ContainerBuilder UseDynamoDb(this ContainerBuilder builder)
    {
        RegisterClassMaps.InitializeDatabase(Database.Value);

        builder.Register(x => new MedicalSpecialityAdapter(Database.Value)).As<IMedicalSpecialityAdapter>();
        builder.Register(x => new PatientAdapter(Database.Value)).As<IPatientAdapter>();
        builder.Register(x => new DoctorAdapter(Database.Value)).As<IDoctorAdapter>();

        return builder;
    }
}
