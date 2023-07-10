using Autofac;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using RuiSantos.ZocDoc.Core.Adapters;
using RuiSantos.ZocDoc.Data.Mongodb.Adapters;
using RuiSantos.ZocDoc.Data.Mongodb.Mappings;
using RuiSantos.ZocDoc.Data.Mongodb.Mappings.Serializers;

namespace RuiSantos.ZocDoc.Data.Mongodb;

public static class MongoExtensions
{
    private static readonly Lazy<IMongoDatabase> database = new(() =>
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Default") ?? "mongodb://localhost:27017/ZocDoc";
        var mongoUrl = MongoUrl.Create(connectionString);
        var client = new MongoClient(mongoUrl);
        return client.GetDatabase(mongoUrl.DatabaseName);
    });

    private static void RegisterClassMaps()
    {
        BsonSerializer.RegisterSerializer(typeof(DateOnly), DateOnlySerializer.Instance);
        BsonSerializer.RegisterSerializer(typeof(TimeOnly), TimeOnlySerializer.Instance);

        var mappings = typeof(IRegisterClassMap).Assembly.GetTypes()
            .Where(t => !t.IsInterface && t.IsAssignableFrom(typeof(IRegisterClassMap)));

        foreach (var mapping in mappings)
        {
            if (Activator.CreateInstance(mapping) is IRegisterClassMap instance)
                instance.Register();
        }
    }

    public static IServiceCollection AddDataContext(this IServiceCollection services)
    {
        RegisterClassMaps();

        services.AddTransient<IMedicalSpecialityAdapter>(x => new MedicalSpecialityAdapter(database.Value));
        services.AddTransient<IPatientAdapter>(x => new PatientAdapter(database.Value));
        services.AddTransient<IDoctorAdapter>(x => new DoctorAdapter(database.Value));

        return services;
    }

    public static ContainerBuilder RegisterDataContext(this ContainerBuilder builder)
    {
        RegisterClassMaps();

        builder.Register(x => new MedicalSpecialityAdapter(database.Value)).As<IMedicalSpecialityAdapter>();
        builder.Register(x => new PatientAdapter(database.Value)).As<IPatientAdapter>();
        builder.Register(x => new DoctorAdapter(database.Value)).As<IDoctorAdapter>();

        return builder;
    }
}

