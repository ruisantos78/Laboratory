namespace RuiSantos.ZocDoc.Core.Tests.Factories;

internal class DoctorBuilder
{
    public static DoctorBuilder As(string license, string email,
        string firstName, string lastName, string specialty) => new(new Doctor()
    {
        License = license,
        Email = email,
        FirstName = firstName,
        LastName = lastName,
        Specialties = new HashSet<string>() { specialty }
    });

    public static DoctorBuilder Dummy(
        string license = "ABC1234",
        string email = "mary.jane@fake.email",
        string firstName = "Mary",
        string lastName = "Jane",
        string specialty = "Cardiology") => As(license, email, firstName, lastName, specialty);

    private readonly Doctor doctor;

    private DoctorBuilder(Doctor doctor)
    {
        this.doctor = doctor ?? new Doctor();
    }

    public Doctor Build() => doctor;
    public List<Doctor> BuildList() => new List<Doctor>() { doctor };

    public DoctorBuilder AddContactNumbers(params string[] contactNumbers)
    {

        doctor.ContactNumbers.UnionWith(contactNumbers);
        return this;
    }

    public DoctorBuilder AddSpecialties(params string[] specialties)
    {
        doctor.Specialties.UnionWith(specialties);
        return this;
    }

    public DoctorBuilder AddOfficeHours(DateTime dateTime)
    {
        doctor.OfficeHours.Add(new OfficeHour(dateTime.DayOfWeek, new[] { dateTime.TimeOfDay }));
        return this;
    }

    public DoctorBuilder AddOfficeHours(DayOfWeek dayOfWeek, params TimeSpan[] hours)
    {
        if (hours.Any())
        {
            doctor.OfficeHours.Add(new OfficeHour(dayOfWeek, hours));
        }

        return this;
    }

    public DoctorBuilder AddOfficeHours(params OfficeHour[] officeHours)
    {
        doctor.OfficeHours.UnionWith(officeHours);
        return this;
    }

    public DoctorBuilder AddAppointments(params DateTime[] dateTime)
    {
        var appointments = dateTime.Select(dt => new Appointment(dt)).ToArray();
        return AddAppointments(appointments);
    }

    public DoctorBuilder AddAppointments(params Appointment[] appointments)
    {
        doctor.Appointments.UnionWith(appointments);
        return this;
    }
}