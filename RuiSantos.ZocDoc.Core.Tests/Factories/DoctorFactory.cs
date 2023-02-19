using RuiSantos.ZocDoc.Core.Models;

namespace RuiSantos.ZocDoc.Core.Tests.Factories;
internal static class DoctorFactory
{
    public static Doctor Empty() => new() { Id = Guid.Empty };

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
        doctor.Specialties = (specialties ?? Array.Empty<string>()).ToList();
        return doctor;
    }

    public static Doctor SetOfficeHours(this Doctor doctor, params OfficeHour[] officeHours)
    {
        doctor.OfficeHours = officeHours?.ToList() ?? new List<OfficeHour>();
        return doctor;
    }

    public static Doctor SetAppointments(this Doctor doctor, params Appointment[] appointments)
    {
        doctor.Appointments = appointments?.ToList() ?? new List<Appointment>();
        return doctor;
    }
}
