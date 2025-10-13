using NSubstitute;
using RuiSantos.Labs.Core.Repositories;
using RuiSantos.Labs.Data.Dynamodb.Adapters;
using RuiSantos.Labs.Data.Dynamodb.Repositories;
using RuiSantos.Labs.Domain.Models;

namespace RuiSantos.Labs.Tests.Asserts.Repositories;

internal class AppointamentsRepositoryAsserts
{
    private readonly IAppointmentAdapter _appointmentAdapter = Substitute.For<IAppointmentAdapter>();
    private readonly IPatientAdapter _patientAdapter = Substitute.For<IPatientAdapter>();

    public IAppointmentsRepository GetRepository() => new AppointmentsRepository(
        _patientAdapter,
        _appointmentAdapter
    );

    public void OnGetPatientAppointmentsAsyncReturns(Doctor doctor, DateOnly date,
        IEnumerable<PatientAppointment> result)
    {
        var patientAppointments = result.ToArray();
        
        var appointments = patientAppointments.Select(x => new Appointment(x.Date))
            .ToArray();

        _appointmentAdapter.LoadByDoctorAsync(doctor, date)
            .Returns(appointments);

        foreach (var appointment in appointments)
        {
            _patientAdapter.GetAppointmentAsync(appointment)
                .Returns(patientAppointments.First(x =>
                    x.Date.DayOfYear == appointment.Date.DayOfYear &&
                    x.Date.Year == appointment.Date.Year &&
                    x.Date.TimeOfDay == appointment.Time)
                );
        }
    }

    public void WhenGetPatientAppointmentsAsyncThrows(Doctor doctor, DateOnly date, Exception ex)
    {
        _appointmentAdapter
            .When(x => x.LoadByDoctorAsync(doctor, date))
            .Throw(ex);
    }
}