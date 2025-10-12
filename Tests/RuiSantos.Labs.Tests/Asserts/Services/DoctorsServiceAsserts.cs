using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using RuiSantos.Labs.Core.Models;
using RuiSantos.Labs.Core.Repositories;
using RuiSantos.Labs.Core.Services;
using RuiSantos.Labs.Domain.Extensions.Models;
using RuiSantos.Labs.Domain.Extensions.Repositories;
using RuiSantos.Labs.Tests.Asserts.Providers;
using RuiSantos.Labs.Tests.Asserts.Repositories;

namespace RuiSantos.Labs.Tests.Asserts.Services;

internal class DoctorsServiceAsserts : LoggerAsserts<DoctorService>
{
    private readonly DoctorRepositoryAsserts _doctorRepository = new();
    private readonly AppointamentsRepositoryAsserts _appointmentsRepository = new();
    private readonly IDoctorService _doctorService;

    public IDoctorService GetService() => _doctorService;

    public DoctorsServiceAsserts()
    {
        var provider = new ServiceCollection()
            .AddLogging(builder => builder.AddSubstitute(Logger))
            .AddTransient<IDoctorRepository>(_ => _doctorRepository.GetRepository())
            .AddTransient<IAppointmentsRepository>(_ => _appointmentsRepository.GetRepository())
            .BuildServiceProvider();
        
        _doctorService = new DoctorService(provider);
    }
    
    public Task ShouldAddAsync(Expression<Predicate<Doctor>> expression, bool received = true)
    {
        var doctor = Arg.Is(expression);
        return _doctorRepository.ShouldStoreAsync(doctor, received);
    }

    public Task ShouldStoreAsync(Expression<Predicate<Doctor>> expression, bool received = true)
    {
        var doctor = Arg.Is(expression);
        return _doctorRepository.ShouldStoreAsync(doctor, received);
    }

    public void OnFindAsyncReturns(Guid doctorId,
        Doctor? result)
    {
        _doctorRepository.OnFindAsyncReturns(doctorId, result);
    }

    public void OnFindAllAsyncReturns(int take, string? paginationToken, Pagination<Doctor> result)
    {
        _doctorRepository.OnFindAllAsyncReturns(take, paginationToken, result);
    }

    public void OnGetPatientAppointmentsAsyncReturns(Doctor doctor, DateOnly date,
        IEnumerable<PatientAppointment> result)
    {
        _appointmentsRepository.OnGetPatientAppointmentsAsyncReturns(doctor, date, result);
    }

    public void WhenAddAsyncThrows(string licence)
    {
        var doctor = Arg.Is<Doctor>(x => x.License == licence);
        _doctorRepository.WhenStoreAsyncThrows(doctor, new Exception());
    }

    public void WhenFindAsyncThrows(Guid doctorId)
    {
        _doctorRepository.WhenFindAsyncThrows(doctorId, new Exception());
    }

    public void WhenGetPatientAppointmentsAsyncThrows(Doctor doctor, DateOnly date)
    {
        _appointmentsRepository.WhenGetPatientAppointmentsAsyncThrows(doctor, date, new Exception());
    }
}