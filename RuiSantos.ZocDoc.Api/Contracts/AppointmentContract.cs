namespace RuiSantos.ZocDoc.Api.Contracts;

/// <summary>
/// Appointment Request
/// </summary>
public class AppointmentContract
{
    /// <summary>
    /// Medical License 
    /// </summary>
    public string MedicalLicense { get; set; }

    /// <summary>
    /// Patient Security Social Number
    /// </summary>
    public string PatientSecuritySocialNumber { get; set; }

    /// <summary>
    /// Appointment Date
    /// </summary>
    public DateTime Date { get; set; }

    public AppointmentContract()
    {
        this.MedicalLicense = string.Empty;
        this.PatientSecuritySocialNumber = string.Empty;
        this.Date = DateTime.MinValue;
    }
}
