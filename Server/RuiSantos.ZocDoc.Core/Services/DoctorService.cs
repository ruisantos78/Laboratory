﻿using Microsoft.Extensions.Logging;
using RuiSantos.ZocDoc.Core.Repositories;
using RuiSantos.ZocDoc.Core.Cache;
using RuiSantos.ZocDoc.Core.Services.Exceptions;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Core.Resources;
using RuiSantos.ZocDoc.Core.Validators;

namespace RuiSantos.ZocDoc.Core.Services;

/// <summary>
/// Manages the creation and modification of doctors.
/// </summary>
public interface IDoctorService
{
    /// <summary>
    /// Creates a new doctor.
    /// </summary>
    /// <param name="license">The doctor's license number.</param>
    /// <param name="email">The doctor's email.</param>
    /// <param name="firstName">The doctor's first name.</param>
    /// <param name="lastName">The doctor's last name.</param>
    /// <param name="contactNumbers">The doctor's contact numbers.</param>
    /// <param name="specialties">The doctor's specialties.</param>
    /// <exception cref="ValidationFailException">Thrown when the doctor's license number is not unique.</exception>
    /// <exception cref="ServiceFailException">Thrown when the operation fails.</exception>
    Task CreateDoctorAsync(string license, string email, string firstName, string lastName, IEnumerable<string> contactNumbers, IEnumerable<string> specialties);

    /// <summary>
    /// Get the doctor's appointments on a given date.
    /// </summary>
    /// <param name="license">The doctor's license number.</param>
    /// <param name="dateTime">The date.</param>
    /// <returns>The doctor's appointments on the given date.</returns>
    Task<IEnumerable<PatientAppointment>> GetAppointmentsAsync(string license, DateTime? dateTime);

    /// <summary>
    /// Get the doctor's informations by a given license number.
    /// </summary>
    /// <param name="license">The doctor's license number.</param>
    /// <returns>The doctor's informations.</returns>
    /// <exception cref="ServiceFailException">Thrown when the operation fails.</exception>
    Task<Doctor?> GetDoctorByLicenseAsync(string license);

    /// <summary>
    /// Set the office hours for a doctor.
    /// </summary>
    /// <param name="license">The doctor's license number.</param>
    /// <param name="dayOfWeek">The day of the week.</param>
    /// <param name="hours">The office hours.</param>
    /// <exception cref="ValidationFailException">Thrown when the doctor's license number is not found.</exception>
    /// <exception cref="ServiceFailException">Thrown when the operation fails.</exception>
    Task SetOfficeHoursAsync(string license, DayOfWeek dayOfWeek, IEnumerable<TimeSpan> hours);
}

internal class DoctorService : IDoctorService
{
    private readonly IRepositoryCache repositoryCache;
    private readonly IDoctorRepository doctorRepository;
    private readonly IPatientRepository patientRepository;
    private readonly IAppointamentsRepository appointamentsRepository;
    private readonly ILogger logger;

    public DoctorService(IRepositoryCache repositoryCache,
                        IDoctorRepository doctorRepository,
                        IPatientRepository patientRepository,
                        IAppointamentsRepository appointamentsRepository,
                        ILogger<DoctorService> logger)
    {
        this.repositoryCache = repositoryCache;
        this.doctorRepository = doctorRepository;
        this.patientRepository = patientRepository;
        this.appointamentsRepository = appointamentsRepository;
        this.logger = logger;
    }

    public async Task CreateDoctorAsync(string license, string email, string firstName, string lastName,
        IEnumerable<string> contactNumbers, IEnumerable<string> specialties)
    {
        try
        {
            var doctor = new Doctor()
            {
                License = license,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                ContactNumbers = contactNumbers.ToHashSet(),
                Specialties = specialties.ToHashSet()
            };

            await ValidateDoctorAsync(doctor);
            await doctorRepository.StoreAsync(doctor);
        }
        catch (ValidationFailException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger?.Fail(ex);
            throw new ServiceFailException(MessageResources.DoctorSetFail);
        }
    }

    public async Task SetOfficeHoursAsync(string license, DayOfWeek dayOfWeek, IEnumerable<TimeSpan> hours)
    {
        try
        {
            var doctor = await doctorRepository.FindAsync(license) ??
                throw new ValidationFailException(MessageResources.DoctorLicenseNotFound);

            // TODO: Only cancel appointment when the hour is excluded.
            doctor.OfficeHours.RemoveWhere(hour => hour.Week == dayOfWeek);
            if (hours.Any())
                doctor.OfficeHours.Add(new OfficeHour(dayOfWeek, hours));

            await CancelAppointmentsAsync(doctor, dayOfWeek, hours);
            await doctorRepository.StoreAsync(doctor);
        }
        catch (ValidationFailException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger?.Fail(ex);
            throw new ServiceFailException(MessageResources.DoctorSetFail);
        }
    }

    public async Task<Doctor?> GetDoctorByLicenseAsync(string license)
    {
        try
        {
            return await doctorRepository.FindAsync(license);
        }
        catch (Exception ex)
        {
            logger?.Fail(ex);
            throw new ServiceFailException(MessageResources.DoctorsGetFail);
        }
    }

    public async Task<IEnumerable<PatientAppointment>> GetAppointmentsAsync(string license, DateTime? dateTime)
    {
        var date = DateOnly.FromDateTime(dateTime ?? DateTime.Today);

        var doctor = await doctorRepository.FindAsync(license);
        if (doctor is null)
            return Array.Empty<PatientAppointment>();

        return await appointamentsRepository.GetPatientAppointmentsAsync(doctor, date);
    }

    private async Task ValidateDoctorAsync(Doctor model)
    {
        var medicalSpecialties = await repositoryCache.GetMedicalSpecialtiesAsync();
        Validator.ThrowExceptionIfIsNotValid(model, medicalSpecialties);
    }

    private Task CancelAppointmentsAsync(Doctor doctor, DayOfWeek dayOfWeek, IEnumerable<TimeSpan> hours)
    {
        // var appointments = doctor.Appointments
        //     .Where(appointment => appointment.Week == dayOfWeek && !hours.Contains(appointment.Time))
        //     .ToHashSet();

        // if (!appointments.Any())
        //     return;

        // var patients = await patientRepository.FindAllWithAppointmentsAsync(appointments);
        // if (patients.Any())
        // {
        //     await Task.WhenAll(patients.Select(p =>
        //     {
        //         p.Appointments.RemoveWhere(pa => appointments.Any(da => da.Id == pa.Id));
        //         return patientRepository.StoreAsync(p);
        //     }).ToArray());
        // }

        // doctor.Appointments.RemoveWhere(item => appointments.Any(a => a.Id == item.Id));

        return Task.CompletedTask;
    }
}
