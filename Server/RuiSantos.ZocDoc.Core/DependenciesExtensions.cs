using Autofac;
using RuiSantos.ZocDoc.Core.Cache;
using RuiSantos.ZocDoc.Core.Services;

namespace RuiSantos.ZocDoc.Core;

/// <summary>
/// Extension methods for registering dependencies in the container.
/// </summary>
public static class DependenciesExtensions
{
    /// <summary>
    /// Registers the services that are used by the application.
    /// </summary>
    /// <param name="container">The container builder.</param>
    public static void UseZocDocServices(this ContainerBuilder container) {
        container.RegisterType<RepositoryCache>().As<IRepositoryCache>().SingleInstance();

        container.RegisterType<MedicalSpecialtiesService>().As<IMedicalSpecialtiesService>().InstancePerDependency();
        container.RegisterType<DoctorService>().As<IDoctorService>().InstancePerDependency();
        container.RegisterType<PatientService>().As<IPatientService>().InstancePerDependency();
        container.RegisterType<AppointmentService>().As<IAppointmentService>().InstancePerDependency();
    }
}