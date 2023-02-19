using RuiSantos.ZocDoc.Core.Models;

namespace RuiSantos.ZocDoc.Api.Contracts;

/// <summary>
/// Contract for a doctor's appointments with a patient
/// </summary>
public class DoctorAppointmentsContract
{
    /// <summary>
    /// The information about the patient
    /// </summary>
    public PatientContract Patient { get; set; }

    /// <summary>
    /// The appointment date
    /// </summary>
    public DateTime Date { get; set; }

    public DoctorAppointmentsContract()
    {
        Patient = new PatientContract();
        Date = DateTime.MinValue;
    }

    public DoctorAppointmentsContract(Patient patient, DateTime date)
    {
        Patient = new PatientContract(patient);
        Date = date;
    }
}
