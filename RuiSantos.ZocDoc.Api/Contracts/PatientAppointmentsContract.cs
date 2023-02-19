using RuiSantos.ZocDoc.Core.Models;

namespace RuiSantos.ZocDoc.Api.Contracts;

/// <summary>
/// Contract for a patient's appointment with a doctor
/// </summary>
public class PatientAppointmentsContract
{
    /// <summary>
    /// The doctor's information
    /// </summary>
    public DoctorContract Doctor { get; set; }

    /// <summary>
    /// The date of the appointment
    /// </summary>
    public DateTime Date { get; set; }

    public PatientAppointmentsContract()
    {
        this.Doctor = new DoctorContract();
        this.Date = DateTime.Now;
    }

    public PatientAppointmentsContract(Doctor doctor, DateTime date)
    {
        this.Doctor = new DoctorContract(doctor);
        this.Date = date;
    }
}
