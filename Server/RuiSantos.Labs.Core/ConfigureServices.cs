using RuiSantos.Labs.Core.Cache;
using RuiSantos.Labs.Core.Services;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for registering dependencies in the container.
/// </summary>
public static class ConfigureServices
{
    public static IServiceCollection AddLabsServices(this IServiceCollection services)
    {
        services.AddSingleton<IRepositoryCache, RepositoryCache>();

        services.AddTransient<IMedicalSpecialtiesService, MedicalSpecialtiesService>();
        services.AddTransient<IDoctorService, DoctorService>();
        services.AddTransient<IPatientService, PatientService>();
        services.AddTransient<IAppointmentService, AppointmentService>();

        return services;
    }
}