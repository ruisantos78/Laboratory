using Autofac;
using RuiSantos.ZocDoc.Core.Data;
using RuiSantos.ZocDoc.Core.Managers;

namespace RuiSantos.ZocDoc.Core;

public static class DependenciesExtensions
{
    public static void RegisterZocDocServices(this ContainerBuilder container) {
        container.RegisterType<DomainContext>().As<IDomainContext>().SingleInstance();

        container.RegisterType<MedicalSpecialtiesManagement>().As<IMedicalSpecialtiesManagement>().InstancePerDependency();
        container.RegisterType<DoctorManagement>().As<IDoctorManagement>().InstancePerDependency();
        container.RegisterType<PatientManagement>().As<IPatientManagement>().InstancePerDependency();
        container.RegisterType<AppointmentManagement>().As<IAppointmentManagement>().InstancePerDependency();
    }
}