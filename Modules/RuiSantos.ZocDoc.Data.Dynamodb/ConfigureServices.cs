using Amazon.DynamoDBv2;
using Amazon.Runtime;
using RuiSantos.ZocDoc.Core.Repositories;
using RuiSantos.ZocDoc.Data.Dynamodb.Mediators;
using RuiSantos.ZocDoc.Data.Dynamodb.Repositories;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
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
}
