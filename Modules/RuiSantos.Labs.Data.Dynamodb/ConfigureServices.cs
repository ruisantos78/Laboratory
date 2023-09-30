using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Microsoft.Extensions.DependencyInjection;
using RuiSantos.Labs.Core.Repositories;
using RuiSantos.Labs.Data.Dynamodb.Mediators;
using RuiSantos.Labs.Data.Dynamodb.Repositories;

namespace RuiSantos.Labs.Data.Dynamodb;

public static class ConfigureServices
{
    public static IServiceCollection AddLabsDynamoDb(this IServiceCollection services, string serviceUrl,
        string accessKey = "api", string secretKey = "secret")
    {
        services.AddSingleton<IAmazonDynamoDB>(_ =>
        {
            var credentials = new BasicAWSCredentials(accessKey, secretKey);
            var config = new AmazonDynamoDBConfig() { ServiceURL = serviceUrl };
            var client = new AmazonDynamoDBClient(credentials, config);

            RegisterClassMaps.InitializeDatabaseAsync(client).Wait();

            return client;
        });

        services.AddTransient<IMedicalSpecialityRepository, MedicalSpecialityRepository>();
        services.AddTransient<IPatientRepository, PatientRepository>();
        services.AddTransient<IDoctorRepository, DoctorRepository>();
        services.AddTransient<IAppointamentsRepository, AppointamentsRepository>();

        return services;
    }
}
