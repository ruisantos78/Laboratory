using System.Linq.Expressions;
using NSubstitute;
using RuiSantos.Labs.Core.Models;
using RuiSantos.Labs.Core.Services;
using RuiSantos.Labs.Tests.Asserts.Repositories;

namespace RuiSantos.Labs.Tests.Asserts.Services;

internal class DoctorsServiceAsserts : LoggerAsserts<DoctorService>
{
    private readonly DoctorRepositoryAsserts _doctorRepository = new();
    private readonly AppointamentsRepositoryAsserts _appointamentsRepository = new();

    public IDoctorService GetService() => new DoctorService(
        _doctorRepository.GetRepository(),
        _appointamentsRepository.GetRepository(),
        Logger
    );

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
        _appointamentsRepository.OnGetPatientAppointmentsAsyncReturns(doctor, date, result);
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
        _appointamentsRepository.WhenGetPatientAppointmentsAsyncThrows(doctor, date, new Exception());
    }
}