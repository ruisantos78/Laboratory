using RuiSantos.Labs.Core.Models;

namespace RuiSantos.Labs.Api.Contracts;

/// <summary>
/// Contract for a doctor's appointments with a patient
/// </summary>
/// <remarks>
/// Constructor for a patient appointment
/// </remarks>
/// <param name="model">The patient appointment</param>
public class DoctorAppointmentsContract(PatientAppointment model)
{

    /// <summary>
    /// The information about the patient
    /// </summary>
    public PatientContract Patient => model.Patient;

    /// <summary>
    /// The appointment date
    /// </summary>
    public DateTime Date => model.Date;

    /// <summary>
    /// Empty constructor for serialization
    /// </summary>
    public DoctorAppointmentsContract() : this(PatientAppointment.Empty)
    {
    }

    public static implicit operator DoctorAppointmentsContract(PatientAppointment model) => new(model);
}
