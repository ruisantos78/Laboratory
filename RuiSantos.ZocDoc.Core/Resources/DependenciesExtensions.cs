using Autofac;
using RuiSantos.ZocDoc.Core.Data;
using RuiSantos.ZocDoc.Core.Managers;

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
    public static void RegisterZocDocServices(this ContainerBuilder container) {
        container.RegisterType<DomainContext>().As<IDomainContext>().SingleInstance();

        container.RegisterType<MedicalSpecialtiesManagement>().As<IMedicalSpecialtiesManagement>().InstancePerDependency();
        container.RegisterType<DoctorManagement>().As<IDoctorManagement>().InstancePerDependency();
        container.RegisterType<PatientManagement>().As<IPatientManagement>().InstancePerDependency();
        container.RegisterType<AppointmentManagement>().As<IAppointmentManagement>().InstancePerDependency();
    }
}