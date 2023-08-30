using RuiSantos.Labs.Core.Models;

namespace RuiSantos.Labs.Api.Contracts;

/// <summary>
/// Contract for a doctor's availability on a specific date
/// </summary>
public class DoctorAvailabilityContract
{
    /// <summary>
    /// The doctor's informations
    /// </summary>
    public DoctorContract Doctor { get; init; }    

    /// <summary>
    /// An array with the availability schedule for the doctor on a specific date
    /// </summary>
    public IEnumerable<DateTime> Schedule { get; init; }

    /// <summary>
    /// Constructor for an empty doctor availability
    /// </summary>
    public DoctorAvailabilityContract() : this(DoctorSchedule.Empty) { }

    /// <summary>
    /// Constructor for a doctor availability
    /// </summary>
    /// <param name="doctorSchedule">The doctor's availability</param>
    public DoctorAvailabilityContract(DoctorSchedule doctorSchedule)
    {
        Doctor =  new DoctorContract(doctorSchedule.Doctor);
        Schedule = doctorSchedule.Schedule;
    }
}
