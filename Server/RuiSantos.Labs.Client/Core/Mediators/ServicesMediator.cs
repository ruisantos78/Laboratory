using System.Reflection;
using Blazing.Mvvm.ComponentModel;

namespace RuiSantos.Labs.Client.Core.Mediators;

internal static class ServicesMediator
{
    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        var dependencies = typeof(ServicesMediator).Assembly.GetTypes()
            .Where(x => x.GetCustomAttribute<RegisterServiceAttribute>() is not null)
            .ToList();

        foreach (var dependency in dependencies)
        {
            dependency.GetCustomAttributes<RegisterServiceAttribute>()
                .ToList()
                .ForEach(attribute => attribute.Register(dependency, services));
        }

        return services;
    }

    public static IServiceCollection RegisterViewModels(this IServiceCollection services)
    {
        typeof(ServicesMediator).Assembly.GetTypes()
            .Where(x => x.IsAssignableTo(typeof(IViewModelBase)))
            .ToList()
            .ForEach(x => services.AddTransient(x));

        return services;
    }
}
