using RuiSantos.Labs.GraphQL;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddLabsGraphQL(this IServiceCollection services)
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

