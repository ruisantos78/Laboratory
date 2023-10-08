using RuiSantos.Labs.Core.Models;

namespace RuiSantos.Labs.Tests.Asserts.Builders;

internal sealed class PatientAppointmentBuilder
{
    private readonly HashSet<PatientAppointment> _appointments = new();

    public HashSet<PatientAppointment> Build() => _appointments;

    public PatientAppointmentBuilder AddAppointment(DateTime dateTime, string? securityNumber = null,
        string? email = null, string? firstName = null, string? lastName = null)
    {
        var patient = new PatientBuilder()
            .With(securityNumber, email, firstName, lastName)
            .Build();

        _appointments.Add(new PatientAppointment(patient, dateTime));

        return this;
    }
}
