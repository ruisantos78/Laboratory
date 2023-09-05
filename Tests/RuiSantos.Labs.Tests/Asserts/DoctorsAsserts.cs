using NSubstitute;
using RuiSantos.Labs.Core.Cache;
using RuiSantos.Labs.Core.Repositories;
using RuiSantos.Labs.Core.Services;

namespace RuiSantos.Labs.Tests.Asserts;

internal class DoctorsAsserts: ServiceAsserts<DoctorService>
{    
    public IDoctorRepository DoctorRepository { get; }
    public IPatientRepository PatientRepository { get; }
    public IAppointamentsRepository AppointamentsRepository { get; }
    
    private IRepositoryCache Cache { get; }

    public DoctorsAsserts(): base()
    {
        Cache = Substitute.For<IRepositoryCache>();
        DoctorRepository = Substitute.For<IDoctorRepository>();
        PatientRepository = Substitute.For<IPatientRepository>();
        AppointamentsRepository = Substitute.For<IAppointamentsRepository>();
    }

    public DoctorsAsserts(IEnumerable<string> specialties) : this()
    {
        Cache.GetMedicalSpecialtiesAsync().Returns(specialties.ToHashSet());
    }
    
    public IDoctorService GetService()
    {
        return new DoctorService(
            Cache,
            DoctorRepository,
            PatientRepository,
            AppointamentsRepository,
            Logger);
    }
}