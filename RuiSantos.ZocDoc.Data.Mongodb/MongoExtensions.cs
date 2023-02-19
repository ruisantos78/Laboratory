using Microsoft.Extensions.DependencyInjection;
using RuiSantos.ZocDoc.Core.Data;

namespace RuiSantos.ZocDoc.Data.Mongodb;

public static class MongoExtensions
{
    public static IServiceCollection AddDataContext(this IServiceCollection services, string? connectionString)
    {
        if (connectionString is null)
            throw new ArgumentNullException(nameof(connectionString));

        services.AddSingleton<IDataContext>(new MongoContext(connectionString));
        return services;
    }
}

