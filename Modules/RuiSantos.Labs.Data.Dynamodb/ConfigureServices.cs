using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RuiSantos.Labs.Core.Repositories;
using RuiSantos.Labs.Data.Dynamodb.Repositories;

namespace RuiSantos.Labs.Data.Dynamodb;

public static class ConfigureServices
{
    public static IServiceCollection AddLabsDynamoDb(this IServiceCollection services, IConfiguration configuration)
    {
        var options = configuration.GetAWSOptions();
        options.Credentials = new EnvironmentVariablesAWSCredentials();

        services.AddDefaultAWSOptions(options);
        services.AddAWSService<IAmazonDynamoDB>();

        services.AddHostedService<DatabaseServices>();

        services.AddTransient<IMedicalSpecialityRepository, MedicalSpecialityRepository>();
        services.AddTransient<IPatientRepository, PatientRepository>();
        services.AddTransient<IDoctorRepository, DoctorRepository>();
        services.AddTransient<IAppointamentsRepository, AppointamentsRepository>();

        return services;
    }
}
