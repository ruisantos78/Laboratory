using RuiSantos.Labs.Core.Models;

namespace RuiSantos.Labs.Api.Contracts;

/// <summary>
/// Contract for a doctor's appointments with a patient
/// </summary>
public class DoctorAppointmentsContract
{
    /// <summary>
    /// The information about the patient
    /// </summary>
    public PatientContract Patient { get; init; }

    /// <summary>
    /// The appointment date
    /// </summary>
    public DateTime Date { get; init; }

    /// <summary>
    /// Empty constructor for serialization
    /// </summary>
    public DoctorAppointmentsContract() : this(PatientAppointment.Empty)
    {
    }

    /// <summary>
    /// Constructor for a patient appointment
    /// </summary>
    /// <param name="patientAppointment">The patient appointment</param>
    public DoctorAppointmentsContract(PatientAppointment patientAppointment)
    {
        Patient = new PatientContract(patientAppointment.Patient);
        Date = patientAppointment.Date;
    }
}
