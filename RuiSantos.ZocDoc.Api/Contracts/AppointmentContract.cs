namespace RuiSantos.ZocDoc.Api.Contracts;

/// <summary>
/// Contract for an appointment request for a patient to meet with a medical professional on a specific date.
/// </summary>
public class AppointmentContract
{
    /// <summary>
    /// The medical license number of the medical professional.
    /// </summary>
    public string MedicalLicense { get; set; }

    /// <summary>
    /// The security social number of the patient.
    /// </summary>
    public string PatientSecuritySocialNumber { get; set; }

    /// <summary>
    /// The date and time of the appointment.
    /// </summary>
    public DateTime Date { get; set; }

    public AppointmentContract()
    {
        MedicalLicense = string.Empty;
        PatientSecuritySocialNumber = string.Empty;
        Date = DateTime.MinValue;
    }
}
