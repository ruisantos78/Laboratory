using RuiSantos.ZocDoc.Core.Models;

namespace RuiSantos.ZocDoc.Api.Contracts;

/// <summary>
/// Patient's appointments contract
/// </summary>
public class PatientAppointmentsContract
{
    /// <summary>
    /// Doctor's informations
    /// </summary>
    public DoctorContract Doctor { get; set; }

    /// <summary>
    /// Appointment's date
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
