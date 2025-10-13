using System;

namespace RuiSantos.Labs.Services.Endpoints.Schemas;

public class StoreDoctorRequest
{
    public string? LicenseNumber { get; init; }
    public string? Email { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string[]? ContactNumbers { get; init; }
    public string[]? Specializations { get; init; }
    public StoreDoctorRequestScheduleItem[]? Avaliability { get; init; }
}

public class StoreDoctorRequestScheduleItem
{
    public DayOfWeek? DayOfWeek { get; init; }
    public TimeSpan[]? TimeSlots { get; init; }
}
