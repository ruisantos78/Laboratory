using System.Linq.Expressions;
using NSubstitute;
using RuiSantos.Labs.Core.Cache;
using RuiSantos.Labs.Core.Models;
using RuiSantos.Labs.Core.Repositories;
using RuiSantos.Labs.Core.Services;

namespace RuiSantos.Labs.Tests.Asserts;

internal class DoctorsAsserts: ServiceAsserts<DoctorService>
{    
    private IDoctorRepository DoctorRepository { get; }
    private IPatientRepository PatientRepository { get; }
    private IAppointamentsRepository AppointamentsRepository { get; }    
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

    public void ThrowOnAddAsync(string licence)
    {
        DoctorRepository
            .When(x => x.StoreAsync(Arg.Is<Doctor>(x => x.License == licence)))
            .Throw<Exception>();
    }

    public async Task ShouldAddAsync(Expression<Predicate<Doctor>> expression)
    {
        await DoctorRepository.Received()
            .StoreAsync(Arg.Is(expression));
    }

    public async Task ShouldStoreAsync(Expression<Predicate<Doctor>> expression)
    {
        await DoctorRepository.Received()
            .StoreAsync(Arg.Is(expression));
    }

    public async Task ShouldNotStoreAsync(Expression<Predicate<Doctor>> expression)
    {
        await DoctorRepository.DidNotReceive()
            .StoreAsync(Arg.Is(expression));
    }

    public void ReturnsOnFindAsync(string license, Doctor? result)
    {
        DoctorRepository.FindAsync(license)
            .Returns(result);
    }
}