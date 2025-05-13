namespace RuiSantos.Labs.Api.Contracts;

/// <summary>
/// Contract for an appointment request for a patient to meet with a medical professional on a specific date.
/// </summary>
public class AppointmentContract()
{
    /// <summary>
    /// The medical license number of the medical professional.
    /// </summary>
    public Guid DoctorId { get; init; } = Guid.Empty;

    /// <summary>
    /// The security social number of the patient.
    /// </summary>
    public string PatientSecuritySocialNumber { get; init; } = string.Empty;

    /// <summary>
    /// The date and time of the appointment.
    /// </summary>
    public DateTime Date { get; init; } = DateTime.MinValue;
}
