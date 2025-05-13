using RuiSantos.Labs.Core.Models;

namespace RuiSantos.Labs.Api.Contracts;

/// <summary>
/// Contract for a doctor's availability on a specific date
/// </summary>
public class DoctorAvailabilityContract(DoctorSchedule model)
{
    /// <summary>
    /// The doctor's informations
    /// </summary>
    public DoctorContract Doctor => model.Doctor;

    /// <summary>
    /// An array with the availability schedule for the doctor on a specific date
    /// </summary>
    public IEnumerable<DateTime> Schedule => model.Schedule;

    /// <summary>
    /// Constructor for an empty doctor availability
    /// </summary>
    public DoctorAvailabilityContract() : this(DoctorSchedule.Empty)
    {
    }

    public static implicit operator DoctorAvailabilityContract(DoctorSchedule model) => new(model);
}
