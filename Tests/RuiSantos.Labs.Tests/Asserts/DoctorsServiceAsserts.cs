using System.Linq.Expressions;
using NSubstitute;
using RuiSantos.Labs.Core.Models;
using RuiSantos.Labs.Core.Repositories;
using RuiSantos.Labs.Core.Services;

namespace RuiSantos.Labs.Tests.Asserts.Services;

internal class DoctorsServiceAsserts: AssertsBase<DoctorService>
{    
    private readonly IDoctorRepository _doctorRepository = Substitute.For<IDoctorRepository>();
    private readonly IAppointamentsRepository _appointamentsRepository = Substitute.For<IAppointamentsRepository>();

    public IDoctorService GetService() => new DoctorService(_doctorRepository, _appointamentsRepository, Logger);
    
    public Task ShouldAddAsync(Expression<Predicate<Doctor>> expression)
    {
        return _doctorRepository.Received()
            .StoreAsync(Arg.Is(expression));
    }

    public Task ShouldStoreAsync(Expression<Predicate<Doctor>> expression)
    {
        return _doctorRepository.Received()
            .StoreAsync(Arg.Is(expression));
    }

    public Task ShouldNotStoreAsync(Expression<Predicate<Doctor>> expression)
    {
        return _doctorRepository.DidNotReceive()
            .StoreAsync(Arg.Is(expression));
    }
    

    public void OnFindAsyncReturns(Guid doctorId,
        Doctor? result)
    {
        _doctorRepository.FindAsync(doctorId)
            .Returns(result);
    }

    public void OnFindAllAsyncReturns(int take, string? paginationToken, Pagination<Doctor> result)
    {
        _doctorRepository.FindAllAsync(take, paginationToken)
            .Returns(result);
    }

    public void OnGetPatientAppointmentsAsyncReturns(Doctor doctor, DateOnly date,
        IEnumerable<PatientAppointment> result)
    {
        _appointamentsRepository.GetPatientAppointmentsAsync(doctor, date)
            .Returns(result.ToAsyncEnumerable());
    }
    
    public void WhenAddAsyncThrows(string licence)
    {
        Expression<Predicate<Doctor>> expression = doctor => doctor.License == licence;

        _doctorRepository
            .When(x => x.StoreAsync(Arg.Is(expression)))
            .Throw<Exception>();
    }

    public void WhenFindAsyncThrows(Guid doctorId)
    {
        _doctorRepository
            .When(x => x.FindAsync(doctorId))
            .Throw<Exception>();
    }

    public void WhenGetPatientAppointmentsAsyncThrows(Doctor doctor, DateOnly date)
    {
        _appointamentsRepository
            .When(x => x.GetPatientAppointmentsAsync(doctor, date))
            .Throw<Exception>();
    }
}