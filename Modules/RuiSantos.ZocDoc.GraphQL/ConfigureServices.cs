using RuiSantos.ZocDoc.GraphQL;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddZocDocGraphQL(this IServiceCollection services)
    {
        services.AddGraphQLServer()
            .AddMutationConventions()
            .AddFiltering()
            .AddSorting()
            .AddMutationType<Mutations>()
            .AddQueryType<Queries>();

        services.AddScoped<IDoctorSchemaAdapter, DoctorSchemaAdapter>();
        services.AddScoped<IMedicalSpecialtySchemaAdapter, MedicalSpecialtySchemaAdapter>();
        
        return services;
    }
}

