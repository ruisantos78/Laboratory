using System.Reflection;
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
            .AddProjections()
            .AddMutationType<Mutations>()
            .AddQueryType<Queries>();

        services.AddAdapters();
        
        return services;
    }

    private static IServiceCollection AddAdapters(this IServiceCollection services)
    {
        typeof(AdapterAttribute).Assembly.GetTypes()
            .Where(x => x.IsInterface && x.GetCustomAttribute<AdapterAttribute>() is not null)
            .Select(x => new {
                Interface = x,
                Instance = x.GetCustomAttribute<AdapterAttribute>()!.InstanceType
            })
            .ToList()
            .ForEach(x => services.AddScoped(x.Interface, x.Instance));

        return services;
    }
}