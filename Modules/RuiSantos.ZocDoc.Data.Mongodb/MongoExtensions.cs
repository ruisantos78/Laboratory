using Autofac;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using RuiSantos.ZocDoc.Core.Adapters;
using RuiSantos.ZocDoc.Data.Mongodb.Adapters;
using RuiSantos.ZocDoc.Data.Mongodb.Mappings;

namespace RuiSantos.ZocDoc.Data;

public static class MongoExtensions
{
    private static readonly Lazy<IMongoDatabase> database = new(() =>
    {
        var connectionString = Environment.GetEnvironmentVariable("DATABASE_MONGO");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Please set the 'DATABASE_MONGO' before start the application.");
        
        var mongoUrl = MongoUrl.Create(connectionString);
        var client = new MongoClient(mongoUrl);
        return client.GetDatabase(mongoUrl.DatabaseName);
    });

    public static IServiceCollection AddDataContext(this IServiceCollection services)
    {
        RegisterClassMaps.InitializeDatabase();

        services.AddTransient<IMedicalSpecialityAdapter>(x => new MedicalSpecialityAdapter(database.Value));
        services.AddTransient<IPatientAdapter>(x => new PatientAdapter(database.Value));
        services.AddTransient<IDoctorAdapter>(x => new DoctorAdapter(database.Value));

        return services;
    }

    public static ContainerBuilder UseMongoDb(this ContainerBuilder builder)
    {
        RegisterClassMaps.InitializeDatabase();

        builder.Register(x => new MedicalSpecialityAdapter(database.Value)).As<IMedicalSpecialityAdapter>();
        builder.Register(x => new PatientAdapter(database.Value)).As<IPatientAdapter>();
        builder.Register(x => new DoctorAdapter(database.Value)).As<IDoctorAdapter>();

        return builder;
    }
}

