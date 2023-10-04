using Microsoft.Extensions.DependencyInjection;
using RuiSantos.Labs.Core.Services;

namespace RuiSantos.Labs.Core;

/// <summary>
/// Extension methods for registering dependencies in the container.
/// </summary>
public static class ConfigureServices
{
    public static IServiceCollection AddLabsServices(this IServiceCollection services)
    {
        services.AddTransient<IMedicalSpecialtiesService, MedicalSpecialtiesService>();
        services.AddTransient<IDoctorService, DoctorService>();
        services.AddTransient<IPatientService, PatientService>();
        services.AddTransient<IAppointmentService, AppointmentService>();

        return services;
    }
}