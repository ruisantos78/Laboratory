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
    public DoctorContract Doctor { get; init; }

    /// <summary>
    /// The date of the appointment
    /// </summary>
    public DateTime Date { get; init; }

    public PatientAppointmentsContract()
    {
        Doctor = DoctorContract.Empty;
        Date = DateTime.Now;
    }

    public PatientAppointmentsContract(Doctor doctor, DateTime date)
    {
        Doctor = new DoctorContract(doctor);
        Date = date;
    }
}
