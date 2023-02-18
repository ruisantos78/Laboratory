using System;
using Microsoft.Extensions.DependencyInjection;
using RuiSantos.ZocDoc.Core.Data;

namespace RuiSantos.ZocDoc.Data.Mongodb;

public static class MongoExtensions
{
    public static IServiceCollection AddDataContext(this IServiceCollection services, MongoSettings? settings)
    {
        if (settings is null)
            throw new ArgumentNullException(nameof(settings));

        services.AddSingleton<IDataContext>(new MongoContext(settings));
        return services;
    }
}

