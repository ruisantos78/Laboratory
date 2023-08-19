using Microsoft.Extensions.DependencyInjection;
using RuiSantos.ZocDoc.Core.Adapters;

namespace RuiSantos.ZocDoc.GraphQL;

public static class GraphQLExtensions
{
    public static IServiceCollection AddZocDocGraphQL(this IServiceCollection services)
    {
        services.AddGraphQLServer()
            .AddMutationConventions()
            .AddFiltering()
            .AddSorting()
            .AddMutationType<Mutations>()
            .AddQueryType<Queries>();

        return services;
    }
}

