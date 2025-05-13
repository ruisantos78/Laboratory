using RuiSantos.Labs.Core.Models;

namespace RuiSantos.Labs.Api.Contracts;

/// <summary>
/// Contract for a patient's appointment with a doctor
/// </summary>
public class PatientAppointmentsContract(DoctorAppointment model)
{
    /// <summary>
    /// The doctor's information
    /// </summary>
    public DoctorContract Doctor => model.Doctor;

    /// <summary>
    /// The date of the appointment
    /// </summary>
    public DateTime Date => model.Date;

    public PatientAppointmentsContract() : this(DoctorAppointment.Empty)
    {
    }

    public static implicit operator PatientAppointmentsContract(DoctorAppointment model) => new(model);
}
