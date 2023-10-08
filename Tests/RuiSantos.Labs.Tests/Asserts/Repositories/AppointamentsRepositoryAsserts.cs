using NSubstitute;
using RuiSantos.Labs.Core.Models;
using RuiSantos.Labs.Core.Repositories;
using RuiSantos.Labs.Data.Dynamodb.Adapters;
using RuiSantos.Labs.Data.Dynamodb.Repositories;

namespace RuiSantos.Labs.Tests.Asserts.Repositories;

internal class AppointamentsRepositoryAsserts
{
    private readonly IAppointmentAdapter _appointmentAdapter = Substitute.For<IAppointmentAdapter>();
    private readonly IPatientAdapter _patientAdapter = Substitute.For<IPatientAdapter>();

    public IAppointamentsRepository GetRepository() => new AppointamentsRepository(_patientAdapter, _appointmentAdapter);

    public void OnGetPatientAppointmentsAsyncReturns(Doctor doctor, DateOnly date,
        IEnumerable<PatientAppointment> result)
    {
        var appointments = result.Select(x => new Appointment(x.Date))
            .ToArray();

        _appointmentAdapter.LoadByDoctorAsync(doctor, date)
            .Returns(appointments);

        foreach (var appointment in appointments)
        {
            _patientAdapter.GetAppointmentAsync(appointment)
                .Returns(result.First(x =>
                    x.Date.DayOfYear == appointment.Date.DayOfYear &&
                    x.Date.Year == appointment.Date.Year &&
                    x.Date.TimeOfDay == appointment.Time));
        }        
    }

    public void WhenGetPatientAppointmentsAsyncThrows(Doctor doctor, DateOnly date, Exception ex)
    {
        _appointmentAdapter
            .When(x => x.LoadByDoctorAsync(doctor, date))
            .Throw(ex);
    }
}