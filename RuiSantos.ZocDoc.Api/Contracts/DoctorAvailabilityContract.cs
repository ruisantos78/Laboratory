using RuiSantos.ZocDoc.Core.Models;

namespace RuiSantos.ZocDoc.Api.Contracts;

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

    public DoctorAvailabilityContract() : this(ZocDoc.Core.Models.Doctor.Empty, Enumerable.Empty<DateTime>()) { }

    public DoctorAvailabilityContract(Doctor doctor, IEnumerable<DateTime> schedule)
    {
        Doctor =  new DoctorContract(doctor);
        Schedule = schedule;
    }
}
