using NSubstitute;
using RuiSantos.Labs.Core.Models;
using RuiSantos.Labs.Core.Repositories;
using RuiSantos.Labs.Data.Dynamodb.Adapters;
using RuiSantos.Labs.Data.Dynamodb.Repositories;

namespace RuiSantos.Labs.Tests.Asserts.Repositories;

internal class DoctorRepositoryAsserts
{
    private readonly IDoctorAdapter _doctorAdapter = Substitute.For<IDoctorAdapter>();
    private readonly IAppointmentAdapter _appointmentAdapter = Substitute.For<IAppointmentAdapter>();

    public IDoctorRepository GetRepository() => new DoctorRepository(_doctorAdapter, _appointmentAdapter);

    public void OnFindAsyncReturns(Guid doctorId, Doctor? result)
    {
        _doctorAdapter.FindAsync(doctorId)
            .Returns(result);
    }

    public void OnFindBySpecialityAsyncReturns(string specialty, IEnumerable<Doctor> result)
    {
        _doctorAdapter.LoadBySpecialtyAsync(specialty)
            .Returns(result.ToAsyncEnumerable());
    }

    public void OnFindBySpecialtyWithAvailabilityAsyncReturns(string specialty, DateOnly date,
        IEnumerable<DoctorSchedule> result)
    {
        _doctorAdapter.LoadBySpecialtyAsync(specialty)
            .Returns(result.Select(x => x.Doctor).ToAsyncEnumerable());

        foreach (var item in result)
        {
            var appointments = item.Schedule.Select(date => new Appointment(date));
            _appointmentAdapter.LoadByDoctorAsync(item.Doctor, date)
                .Returns(appointments.ToAsyncEnumerable());
        }
    }

    public void OnFindByAppointmentsAsyncReturns(IDictionary<Appointment, Doctor> appointments)
    {
        foreach (var appointment in appointments)
        {
            _doctorAdapter.FindByAppointmentAsync(appointment.Key)
                .Returns(appointment.Value);
        }
    }

    public void OnFindAllAsyncReturns(int take, string? paginationToken, Pagination<Doctor> result)
    {
        _doctorAdapter.LoadByLicenseAsync(take, paginationToken)
            .Returns(result);
    }

    public void OnFindByLicenseAsyncReturns(string license, Doctor? result)
    {
        _doctorAdapter.FindByLicenseAsync(license)
            .Returns(result);
    }

    public Task ShouldStoreAsync(Doctor doctor, bool received = true)
    {
        return received
            ? _doctorAdapter.Received().StoreAsync(doctor)
            : _doctorAdapter.DidNotReceive().StoreAsync(doctor);
    }

    public void WhenStoreAsyncThrows(Doctor doctor, Exception ex)
    {
        _doctorAdapter.When(x => x.StoreAsync(doctor))
            .Throw(ex);
    }

    public void WhenFindAsyncThrows(Guid doctorId, Exception ex)
    {
        _doctorAdapter.When(x => x.FindAsync(doctorId))
            .Throw(ex);
    }
}