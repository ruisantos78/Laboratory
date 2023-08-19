namespace RuiSantos.ZocDoc.Core.Tests.Factories;

internal class PatientBuilder
{
    public static PatientBuilder As(string socialNumber, string email, string firstName, string lastName) =>
        new(new Patient()
        {
            SocialSecurityNumber = socialNumber,
            Email = email,
            FirstName = firstName,
            LastName = lastName
        });

    public static PatientBuilder Dummy(
        string socialNumber = "123-45-6789",
        string email = "jhon.doe@fake.email",
        string firstName = "Jhon",
        string lastName = "Doe") => As(socialNumber, email, firstName, lastName);

    private readonly Patient patient;

    public PatientBuilder(Patient patient)
    {
        this.patient = patient ?? new Patient();
    }

    public Patient Build() => patient;

    public List<Patient> BuildList() => new List<Patient> { patient };

    public PatientBuilder AddAppointments(params DateTime[] dateTime)
    {
        var appointments = dateTime.Select(dt => new Appointment(dt)).ToArray();
        return AddAppointments(appointments);
    }

    public PatientBuilder AddAppointments(params Appointment[] appointments)
    {
        patient.Appointments.UnionWith(appointments);
        return this;
    }
}