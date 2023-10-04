using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using RuiSantos.Labs.GraphQL.Adapters;
using RuiSantos.Labs.GraphQL.Services;
using Mutation = RuiSantos.Labs.GraphQL.Mutations.Mutation;
using Query = RuiSantos.Labs.GraphQL.Queries.Query;

namespace RuiSantos.Labs.GraphQL;

public static class ConfigureServices
{
    public static IServiceCollection AddLabsGraphql(this IServiceCollection services)
    {
        services.AddGraphQLServer()
            .AddMutationConventions()
            .AddFiltering()
            .AddSorting()
            .AddProjections()
            .AddMutationType<Mutation>()
            .AddQueryType<Query>();

        services.AddAdapters();
        services.AddSingleton<ISecurity, Security>();

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