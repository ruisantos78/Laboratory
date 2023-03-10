namespace RuiSantos.ZocDoc.Core.Models;

/// <summary>
/// Represents a patient appointment.
/// </summary>
public struct PatientAppointment {
    /// <summary>
    /// Represents an empty patient appointment.
    /// </summary>
    public static readonly PatientAppointment Empty = new PatientAppointment(Patient.Empty, DateTime.MinValue);

    /// <summary>
    /// The patient.
    /// </summary>
    public Patient Patient { get; }

    /// <summary>
    /// The date.
    /// </summary>
    public DateTime Date { get; }

    /// <summary>
    /// Creates a new patient appointment.
    /// </summary>
    /// <param name="patient">The patient.</param>
    /// <param name="date">The date.</param>
    public PatientAppointment(Patient patient, DateTime date)
    {
        Patient = patient;
        Date = date;
    }
}