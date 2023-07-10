using Amazon.DynamoDBv2;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using RuiSantos.ZocDoc.Core.Adapters;
using RuiSantos.ZocDoc.Data.Dynamodb.Adapters;

namespace RuiSantos.ZocDoc.Data.Dynamodb;

public static class DynamoExtensions
{
    private static readonly Lazy<AmazonDynamoDBClient> Database = new(() => {
        var config = new AmazonDynamoDBConfig
        {
            ServiceURL = Environment.GetEnvironmentVariable("ConnectionStrings__Default") ?? "http://localhost:8000"
        };
        return new AmazonDynamoDBClient(config);            
    });
    
    public static IServiceCollection AddDataContext(this IServiceCollection services)
    {
       // services.AddTransient<IMedicalSpecialityAdapter>(x => new MedicalSpecialityAdapter(database.Value));
       // services.AddTransient<IPatientAdapter>(x => new PatientAdapter(database.Value));
        services.AddTransient<IDoctorAdapter>(x => new DoctorAdapter(Database.Value));

        return services;
    }

    public static ContainerBuilder RegisterDataContext(this ContainerBuilder builder)
    {
       // builder.Register(x => new MedicalSpecialityAdapter(database.Value)).As<IMedicalSpecialityAdapter>();
       // builder.Register(x => new PatientAdapter(database.Value)).As<IPatientAdapter>();
        builder.Register(x => new DoctorAdapter(Database.Value)).As<IDoctorAdapter>();

        return builder;
    }
}
