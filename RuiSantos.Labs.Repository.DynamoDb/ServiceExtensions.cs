using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RuiSantos.Labs.Domain.Repositories;
using RuiSantos.Labs.Repository.DynamoDb.Core;
using RuiSantos.Labs.Repository.DynamoDb.Repositories;

namespace RuiSantos.Labs.Repository.DynamoDb;

public static class ServiceExtensions
{
    public static IServiceCollection AddLabsDynamoDbReporitories(this IServiceCollection services, IConfiguration configuration)
    {
        var options = configuration.GetAWSOptions();
        options.Credentials = new EnvironmentVariablesAWSCredentials();

        services.AddDefaultAWSOptions(options);
        services.AddAWSService<IAmazonDynamoDB>();

        services.AddScoped<IDoctorRepository, DoctorRepository>();
        
        return services;
    }

    public static async Task EnsureTablesExistAsync(this IHost app)
    {
        var amazonDynamoDb = app.Services.GetRequiredService<IAmazonDynamoDB>();
        await ClassMap.CreateTablesAsync(amazonDynamoDb);
    }
}