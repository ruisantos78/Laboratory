using RuiSantos.ZocDoc.Core.Models;

namespace RuiSantos.ZocDoc.Core.Tests.Factories;
internal static class DoctorFactory
{
    public static Doctor Empty() => new();

    public static Doctor Create(
        string medicalLicense = "ABC123",
        string email = "mary@doctor.com",
        string firstName = "Mary",
        string lastName = "Jane",
        string[]? contactNumbers = null
    )
    => new(medicalLicense, email, firstName, lastName, contactNumbers ?? Array.Empty<string>(), new[] { "Cardiology" });

    public static Doctor SetSpecialties(this Doctor doctor, params string[] specialties)
    {
        doctor.Specialties.Clear();

        if (specialties.Any())
            doctor.Specialties.AddRange(specialties);

        return doctor;
    }

    public static Doctor SetOfficeHours(this Doctor doctor, params OfficeHour[] officeHours)
    {
        doctor.OfficeHours.Clear();

        if (officeHours.Any())
            doctor.OfficeHours.AddRange(officeHours);

        return doctor;
    }

    public static Doctor SetAppointments(this Doctor doctor, params Appointment[] appointments)
    {
        doctor.Appointments.Clear();

        if (appointments.Any())
            doctor.Appointments.AddRange(appointments);

        return doctor;
    }
}
