using RuiSantos.Labs.Core.Models;

namespace RuiSantos.Labs.Tests;

public class PatientAppointmentBuilder
{
    private readonly HashSet<PatientAppointment> _appointments = new();

    public IEnumerable<PatientAppointment> Build()
    {
        return _appointments.ToArray();
    }

    public PatientAppointmentBuilder AddAppointment(DateTime dateTime, string securityNumber,
        string? email = null, string? firstName = null, string? lastName = null)
    {
        var patient = new Patient() {
            SocialSecurityNumber = securityNumber,
            FirstName = firstName ?? "Joe",
            LastName = lastName ?? "Doe",
            Email = email ?? "joe.doe@email.com"
        };

        _appointments.Add(new() { 
            Patient = patient, 
            Date = dateTime 
        });

        return this;
    }
}
