namespace RuiSantos.ZocDoc.Core.Models;

/// <summary>
/// Represents a doctor appointment.
/// </summary>
public class DoctorAppointment {
    /// <summary>
    /// Represents an empty doctor appointment.
    /// </summary>
    public static readonly DoctorAppointment Empty = new(Doctor.Empty, DateTime.MinValue);

    /// <summary>
    /// The doctor.
    /// </summary>
    public Doctor Doctor { get; }

    /// <summary>
    /// The date.
    /// </summary>
    public DateTime Date { get; }

    /// <summary>
    /// Creates a new doctor appointment.
    /// </summary>
    /// <param name="doctor">The doctor.</param>
    /// <param name="date">The date.</param>
    public DoctorAppointment(Doctor doctor, DateTime date) {
        Doctor = doctor;
        Date = date;
    }
}