using RuiSantos.ZocDoc.Core.Models;

namespace RuiSantos.ZocDoc.Core.Tests.Factories;

internal static class AppointmentsFactory
{
    static readonly DateTime StartTime = new(2022, 1, 3, 8, 0, 0);
    public static IEnumerable<Appointment> Create(int elements = 1)
        => Enumerable.Range(0, elements).Select(i => new Appointment(StartTime.AddHours(i)));
}
