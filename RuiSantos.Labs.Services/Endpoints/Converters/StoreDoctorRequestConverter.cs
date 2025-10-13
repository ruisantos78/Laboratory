using RuiSantos.Labs.Domain.Models;
using RuiSantos.Labs.Services.Endpoints.Schemas;

namespace RuiSantos.Labs.Services.Endpoints.Converters;

internal static class StoreDoctorRequestConverter
{
    public static Doctor ToDoctor(this StoreDoctorRequest request)
    {
        return new Doctor
        {
            License = request.LicenseNumber ?? string.Empty,
            Email = request.Email ?? string.Empty,
            FirstName = request.FirstName ?? string.Empty,
            LastName = request.LastName ?? string.Empty,
            ContactNumbers = [.. request.ContactNumbers ?? []],
            Specializations = [.. request.Specializations ?? []],
            OfficeHours = [.. request.Availability?.Select(ToOfficeHour) ?? []]
        };
    }

    private static OfficeHour ToOfficeHour(this StoreDoctorRequestScheduleItem request)
    {
        return new OfficeHour
        {
            Week = request.DayOfWeek ?? DayOfWeek.Monday,
            Hours = request.TimeSlots ?? []
        };
    }
}
