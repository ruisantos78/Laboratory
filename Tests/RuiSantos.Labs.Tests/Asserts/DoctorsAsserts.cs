using System.Linq.Expressions;
using NSubstitute;
using RuiSantos.Labs.Core.Models;
using RuiSantos.Labs.Core.Repositories;
using RuiSantos.Labs.Core.Services;

namespace RuiSantos.Labs.Tests.Asserts;

internal class DoctorsAsserts: ServiceAsserts<DoctorService>
{    
    private IDoctorRepository DoctorRepository { get; }
    private IAppointamentsRepository AppointamentsRepository { get; }    

    public DoctorsAsserts(): base()
    {
        DoctorRepository = Substitute.For<IDoctorRepository>();
        AppointamentsRepository = Substitute.For<IAppointamentsRepository>();
    }
    
    public IDoctorService GetService()
    {
        return new DoctorService(
            DoctorRepository,
            AppointamentsRepository,
            Logger);
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

    public void ReturnsOnFindAsync(string license, 
        Doctor? result)
    {
        DoctorRepository.FindAsync(license)
            .Returns(result);
    }

    public void ReturnsOnGetPatientAppointmentsAsync(Doctor doctor, DateOnly date, 
        IEnumerable<PatientAppointment> result)
    {
        AppointamentsRepository.GetPatientAppointmentsAsync(doctor, date)
            .Returns(result);
    }

    public void ThrowOnAddAsync(string licence)
    {
        DoctorRepository
            .When(x => x.StoreAsync(Arg.Is<Doctor>(x => x.License == licence)))
            .Throw<Exception>();
    }

    public void ThrowOnFindAsync(string licence)
    {
        DoctorRepository
            .When(x => x.FindAsync(licence))
            .Throw<Exception>();
    }

    public void ThrowsOnGetPatientAppointmentsAsync(Doctor doctor, DateOnly date)
    {
        AppointamentsRepository
            .When(x => x.GetPatientAppointmentsAsync(doctor, date))
            .Throw<Exception>();
    }
}