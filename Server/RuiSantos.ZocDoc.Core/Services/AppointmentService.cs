using Microsoft.Extensions.Logging;
using RuiSantos.ZocDoc.Core.Repositories;
using RuiSantos.ZocDoc.Core.Services.Exceptions;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Core.Resources;
using RuiSantos.ZocDoc.Core.Validators;

namespace RuiSantos.ZocDoc.Core.Services;

/// <summary>
/// Appointment management interface.
/// </summary>
public interface IAppointmentService
{
    /// <summary>
    /// Gets the appointments for a given patient.
    /// </summary>
    /// <param name="socialNumber">The social number of the patient.</param>
    /// <param name="medicalLicence">The medical licence of the doctor.</param>
    /// <param name="dateTime">The date and time of the appointment.</param>
    /// <exception cref="ValidationFailException">Thrown when the validation fails.</exception>
    /// <exception cref="ManagementFailException">Thrown when the appointment creation fails.</exception>
    Task CreateAppointmentAsync(string socialNumber, string medicalLicence, DateTime dateTime);

    /// <sumary>
    /// Deletes an appointment for a given patient.
    /// </sumary>
    /// <param name="socialNumber">The social number of the patient.</param>
    /// <param name="medicalLicence">The medical licence of the doctor.</param>
    /// <param name="dateTime">The date and time of the appointment.</param>
    /// <exception cref="ValidationFailException">Thrown when the validation fails.</exception>
    /// <exception cref="ManagementFailException">Thrown when the appointment deletion fails.</exception>
    Task DeleteAppointmentAsync(string socialNumber, string medicalLicence, DateTime dateTime);

    /// <summary>
    /// Gets the availability of a doctor.
    /// </summary>
    /// <param name="speciality">Speciality of the doctor.</param>
    /// <param name="dateTime">Date and time of the availability.</param>
    /// <returns>An asynchronous list of DoctorSchedule.</returns>
    IAsyncEnumerable<DoctorSchedule> GetAvailabilityAsync(string speciality, DateTime dateTime);
}

internal class AppointmentService : IAppointmentService
{
    private readonly IDoctorRepository doctorRepository;
    private readonly IPatientRepository patientRepository;
    private readonly IAppointamentsRepository appointamentsRepository;
    private readonly ILogger logger;

    public AppointmentService(IDoctorRepository doctorRepository,
        IPatientRepository patientRepository,
        IAppointamentsRepository appointamentsRepository,
        ILogger<AppointmentService> logger)
    {
        this.doctorRepository = doctorRepository;
        this.patientRepository = patientRepository;
        this.appointamentsRepository = appointamentsRepository;
        this.logger = logger;
    }

    public async Task CreateAppointmentAsync(string socialNumber, string medicalLicence, DateTime dateTime)
    {
        try
        {
            var doctor = await doctorRepository.FindAsync(medicalLicence);
            if (doctor is null)
                throw new ValidationFailException(MessageResources.DoctorLicenseNotFound);

            var doctorAppointment = await appointamentsRepository.GetAsync(doctor, dateTime);
            if (doctorAppointment is not null)
                throw new ValidationFailException(MessageResources.RecordAlreadyExists);

            var patient = await patientRepository.FindAsync(socialNumber);
            if (patient is null)
                throw new ValidationFailException(MessageResources.PatientSocialNumberNotFound);

            var patientAppointment = await appointamentsRepository.GetAsync(patient, dateTime);
            if (patientAppointment is not null)
                throw new ValidationFailException(MessageResources.RecordAlreadyExists);

            var appointment = new Appointment(dateTime);
            await appointamentsRepository.StoreAsync(doctor, patient, dateTime);        
        }        
        catch (ValidationFailException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger?.Fail(ex);
            throw new ServiceFailException(MessageResources.PatientSetFail);
        }
    }

    public async Task DeleteAppointmentAsync(string socialNumber, string medicalLicence, DateTime dateTime)
    {
        try
        {
            var patient = await patientRepository.FindAsync(socialNumber);
            if (patient is null)
                throw new ValidationFailException(MessageResources.PatientSocialNumberNotFound);

            var patientAppointment = await appointamentsRepository.GetAsync(patient, dateTime);
            if (patientAppointment is null)
                return;

            var doctor = await doctorRepository.FindAsync(medicalLicence);
            if (doctor is null)
                return;

            var doctorAppointment = await appointamentsRepository.GetAsync(doctor, dateTime);
            if (doctorAppointment is null)
                return;

            if (patientAppointment.Id != doctorAppointment.Id)
                return;

            await appointamentsRepository.RemoveAsync(patientAppointment);
        }         
        catch (ValidationFailException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger?.Fail(ex);
            throw new ServiceFailException(MessageResources.PatientSetFail);
        }
    }

    public IAsyncEnumerable<DoctorSchedule> GetAvailabilityAsync(string speciality, DateTime dateTime)
    {
        var date = DateOnly.FromDateTime(dateTime);
        return doctorRepository.FindBySpecialtyWithAvailabilityAsync(speciality, date);
    }
}