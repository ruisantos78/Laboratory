namespace RuiSantos.Labs.Client.Core.Mediators;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
internal class RegisterServiceAttribute : Attribute
{
    public InstanceType InstanceType { get; init; } = InstanceType.Transient;

    public Type? ImplementationType { get; init; }

    public RegisterServiceAttribute()
    {
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
