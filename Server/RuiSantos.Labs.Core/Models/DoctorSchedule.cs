namespace RuiSantos.Labs.Core.Models;

/// <summary>
/// A doctor's schedule
/// </summary>
public record DoctorSchedule
{
    /// <summary>
    /// An empty doctor schedule
    /// </summary>
    public static readonly DoctorSchedule Empty = new(Doctor.Empty, Array.Empty<DateTime>());

    /// <summary>
    /// The doctor
    /// </summary>
    public Doctor Doctor { get; }

    /// <summary>
    /// The schedule
    /// </summary>
    public IReadOnlyList<DateTime> Schedule { get; }

    /// <summary>
    /// Creates a new doctor schedule
    /// </summary>
    /// <param name="doctor">The doctor</param>
    /// <param name="schedule">The schedule</param>
    public DoctorSchedule(Doctor doctor, IEnumerable<DateTime> schedule)
    {
        Doctor = doctor;
        Schedule = [.. schedule];
    }
}