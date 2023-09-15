using System.Reflection;

// ReSharper disable once CheckNamespace
namespace RuiSantos.Labs.Client;

internal static class RegisterServiceMediator
{
    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        var dependencies = typeof(RegisterServiceAttribute).Assembly.GetTypes()
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
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
internal class RegisterServiceAttribute: Attribute
{
    public InstanceType InstanceType { get; init; } = InstanceType.Transient;

    public Type? ImplementationType { get; init; }

    public RegisterServiceAttribute() { 
        ImplementationType = null;
    }

    public RegisterServiceAttribute(Type implementationType)
    {
        ImplementationType = implementationType;
    } 

    public IServiceCollection Register(Type dependency, IServiceCollection services)
    {
        return InstanceType switch
        {
            InstanceType.Singleton => ImplementationType is null
                ? services.AddSingleton(dependency)
                : services.AddSingleton(dependency, ImplementationType),

            InstanceType.Scoped => ImplementationType is null
                ? services.AddScoped(dependency)
                : services.AddScoped(dependency, ImplementationType),

            InstanceType.Transient => ImplementationType is null
                ? services.AddTransient(dependency)
                : services.AddTransient(dependency, ImplementationType),

            _ => services
        };
    }         
}

public enum InstanceType 
{
    Transient,
    Scoped,
    Singleton
}