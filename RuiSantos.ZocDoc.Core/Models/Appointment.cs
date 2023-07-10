namespace RuiSantos.ZocDoc.Core.Models;

/// <summary>
/// Represents an appointment.
/// </summary>
public class Appointment
{
    /// <summary>
    /// The unique identifier of the appointment.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// The day of the week the appointment is scheduled.
    /// </summary>
    public DayOfWeek Week { get; init; }

    /// <summary>
    /// The date of the appointment.
    /// </summary>
    public DateOnly Date { get; init; }

    /// <summary>
    /// The time of the appointment.
    /// </summary>
    public TimeSpan Time { get; init; }


    /// <summary>
    /// Create a new instance of the appointment.
    /// </summary>
    public Appointment()
    {
        Id = Guid.NewGuid();
        Week = DayOfWeek.Monday;
        Date = DateOnly.MinValue;
        Time = TimeSpan.MinValue;
    }

    /// <summary>
    /// Creates a new instance of the appointment.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="date">The date of the appointment.</param>
    public Appointment(Guid id, DateTime date)
    {
        Id = id;
        Week = date.DayOfWeek;
        Date = DateOnly.FromDateTime(date);
        Time = date.TimeOfDay;
    }

    /// <summary>
    /// Creates a new instance of the appointment.
    /// </summary>
    /// <param name="date">The date of the appointment.</param>
    public Appointment(DateTime date): this(Guid.NewGuid(), date)
    {
    }

    /// <summary>
    /// Gets the date and time of the appointment.
    /// </summary>
    /// <returns>The date and time of the appointment.</returns>
    public DateTime GetDateTime() => Date.WithTime(Time);
}

