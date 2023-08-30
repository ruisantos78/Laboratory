using RuiSantos.Labs.Core.Models;

namespace RuiSantos.Labs.Api.Contracts;

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

    public PatientAppointmentsContract(): this(DoctorAppointment.Empty) { }

    public PatientAppointmentsContract(DoctorAppointment doctorAppointment)
    {
        Doctor = new DoctorContract(doctorAppointment.Doctor);
        Date = doctorAppointment.Date;
    }
}
