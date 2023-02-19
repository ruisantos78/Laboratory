using RuiSantos.ZocDoc.Core.Models;
using System.Runtime.CompilerServices;

namespace RuiSantos.ZocDoc.Core.Tests.Core;

internal static class ModelsFactory
{
    public static Patient CreatePatient(
        string socialNumber = "123-45-6789",
        string email = "john@patient.com",
        string firstName = "Jhon",
        string lastName = "Doe",
        string[]? contactNumbers = null
    )
    => new(socialNumber, email, firstName, lastName, contactNumbers ?? Array.Empty<string>());

    public static Doctor CreateDoctor(
        string medicalLicense = "ABC123",
        string email = "mary@doctor.com",
        string firstName = "Mary",
        string lastName = "Jane",
        string[]? contactNumbers = null
    )
    => new(medicalLicense, email, firstName, lastName, contactNumbers ?? Array.Empty<string>(), new[] { "Cardiology" });

    public static Doctor SetSpecialties(this Doctor doctor, params string[] specialties)
    {
        doctor.Specialties = (specialties ?? Array.Empty<String>()).ToList();
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
