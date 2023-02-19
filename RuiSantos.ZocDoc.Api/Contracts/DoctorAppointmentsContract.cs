using RuiSantos.ZocDoc.Core.Models;

namespace RuiSantos.ZocDoc.Api.Contracts;

/// <summary>
/// Doctor's appointments contract
/// </summary>
public class DoctorAppointmentsContract
{
    /// <summary>
    /// Patient's information
    /// </summary>
    public PatientContract Patient { get; set; }

    /// <summary>
    /// Appointment date
    /// </summary>
    public DateTime Date { get; set; }

    public DoctorAppointmentsContract()
    {
        this.Patient = new PatientContract();
        this.Date = DateTime.MinValue;
    }

    public DoctorAppointmentsContract(Patient patient, DateTime date)
    {
        this.Patient = new PatientContract(patient);
        this.Date = date;
    }
}
