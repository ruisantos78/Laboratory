using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using RuiSantos.ZocDoc.Core.Adapters;
using RuiSantos.ZocDoc.Core.Managers;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Core.Tests.Factories;

namespace RuiSantos.ZocDoc.Core.Tests;

/// <summary>
/// Tests for <see cref="AppointmentManagement"/>
/// </summary>
public class AppointmentManagementTests
{
    private readonly Mock<IDoctorAdapter> doctorAdapterMock = new();
    private readonly Mock<IPatientAdapter> patientAdapterMock = new();

    /// <summary>
    /// Mocks the logger.
    /// </summary>
    private readonly Mock<ILogger<AppointmentManagement>> loggerMock = new();

    /// <summary>
    /// Tests that <see cref="AppointmentManagement.CreateAppointmentAsync(string, string, DateTime)"/>
    /// store an appointment for a valid patient and doctor. 
    /// </summary>
    [Fact]
    public async Task CreateAppointmentAsync_WithValidInput_ShouldStoreAppointment()
    {
        // Arrange
        var socialNumber = "123-45-6789";
        var medicalLicence = "ABC123";
        var dateTime = DateTime.Parse("2022-01-01 08:00");

        var officeHours = new[] { new OfficeHour(dateTime.DayOfWeek, new[] { dateTime.TimeOfDay }) };

        var patient = PatientFactory.Create(socialNumber);
        var doctor = DoctorFactory.Create(medicalLicence).SetOfficeHours(officeHours);

        patientAdapterMock.Setup(m => m.FindAsync(patient.SocialSecurityNumber)).ReturnsAsync(patient);
        patientAdapterMock.Setup(m => m.StoreAsync(It.IsAny<Patient>())).Verifiable();

        doctorAdapterMock.Setup(m => m.FindAsync(doctor.License)).ReturnsAsync(doctor);
        doctorAdapterMock.Setup(m => m.StoreAsync(It.IsAny<Doctor>())).Verifiable();

        var appointmentManagement = new AppointmentManagement(doctorAdapterMock.Object, patientAdapterMock.Object, loggerMock.Object);

        // Act
        await appointmentManagement.CreateAppointmentAsync(socialNumber, medicalLicence, dateTime);

        // Assert
        patient.Appointments.Should().ContainSingle(appointment => appointment.GetDateTime() == dateTime);
        doctor.Appointments.Should().ContainSingle(appointment => appointment.GetDateTime() == dateTime);

        patientAdapterMock.Verify(m => m.StoreAsync(patient), Times.Once);
        doctorAdapterMock.Verify(m => m.StoreAsync(doctor), Times.Once);
    }

    /// <summary>
    /// Tests that <see cref="AppointmentManagement.CreateAppointmentAsync(string, string, DateTime)"/>
    /// remove an appointment for a valid patient and doctor.
    /// </summary>
    [Fact]
    public async Task DeleteAppointmentAsync_WithValidInput_ShouldRemoveAppointment()
    {
        // Arrange
        var socialNumber = "123-45-6789";
        var medicalLicence = "ABC123";
        var dateTime = DateTime.Parse("2022-01-01 08:00");

        var patient = PatientFactory.Create(socialNumber);
        var doctor = DoctorFactory.Create(medicalLicence);

        var appointment = new Appointment(dateTime);
        patient.Appointments.Add(appointment);
        doctor.Appointments.Add(appointment);

        patientAdapterMock.Setup(m => m.FindAsync(patient.SocialSecurityNumber)).ReturnsAsync(patient);
        patientAdapterMock.Setup(m => m.StoreAsync(It.IsAny<Patient>())).Verifiable();

        doctorAdapterMock.Setup(m => m.FindAsync(doctor.License)).ReturnsAsync(doctor);
        doctorAdapterMock.Setup(m => m.StoreAsync(It.IsAny<Doctor>())).Verifiable();

        var appointmentManagement = new AppointmentManagement(doctorAdapterMock.Object, patientAdapterMock.Object, loggerMock.Object);

        // Act
        await appointmentManagement.DeleteAppointmentAsync(socialNumber, medicalLicence, dateTime);

        // Assert
        patient.Appointments.Should().BeEmpty();
        doctor.Appointments.Should().BeEmpty();

        patientAdapterMock.Verify(m => m.StoreAsync(patient), Times.AtMostOnce);
        doctorAdapterMock.Verify(m => m.StoreAsync(doctor), Times.AtMostOnce);
    }

    /// <summary>
    /// Tests that <see cref="AppointmentManagement.GetAvailabilityAsync(string, DateTime)"/>
    /// returns the availability for a valid doctor.
    /// </summary>
    [Fact]
    public async Task GetAvailabilityAsync_WithValidInput_ReturnsExpectedResult()
    {
        // Arrange
        var speciality = "cardiology";
        var dateTime = DateTime.Parse("2022-01-03 09:00");

        var hours = new[] {
            new TimeSpan(9, 0, 0),
            new TimeSpan(10, 0, 0),
            new TimeSpan(11, 0, 0),
            new TimeSpan(12, 0, 0)
        };

        var officeHours = new[] {
            new OfficeHour(DayOfWeek.Monday, hours),
            new OfficeHour(DayOfWeek.Tuesday, hours)
        };

        var appointments = new[]
        {
            new Appointment(DateTime.Parse("2022-01-03 09:00"))
        };
            
        doctorAdapterMock.Setup(m => m.FindBySpecialtyWithAvailabilityAsync(speciality, DateOnly.FromDateTime(dateTime)))
            .ReturnsAsync(new List<Doctor>()
            {
                DoctorFactory.Create("001").SetSpecialties(speciality).SetOfficeHours(officeHours).SetAppointments(appointments)
            });

        var appointmentManagement = new AppointmentManagement(doctorAdapterMock.Object, patientAdapterMock.Object, loggerMock.Object);

        // Act
        var result = await appointmentManagement.GetAvailabilityAsync(speciality, dateTime).ToListAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
        result.Should().ContainSingle().Which.Doctor.License.Should().Be("001");
        result.Should().ContainSingle().Which.Schedule.Should().HaveCount(3);
        result.Should().ContainSingle().Which.Schedule.Should().NotContain(dateTime);

        doctorAdapterMock.Verify();
    }

    /// <summary>
    /// This test is to ensure that when there are no doctors with the given speciality,
    /// the result is empty.
    /// </summary>
    [Fact]
    public async Task GetAvailabilityAsync_WithInvalidInput_ReturnsExpectedResult() {
        /// Arrange
        var speciality = "cardiology";
        var dateTime = DateTime.Parse("2022-01-03 09:00");

        var appointments = new[] {
            new Appointment(DateTime.Parse("2022-01-04 09:00"))
        };

        doctorAdapterMock.Setup(m => m.FindBySpecialtyWithAvailabilityAsync(speciality, DateOnly.FromDateTime(dateTime)))
            .ReturnsAsync(new List<Doctor>());

        var management = new AppointmentManagement(doctorAdapterMock.Object, patientAdapterMock.Object, loggerMock.Object);       
        
        // Act
        var result = await management.GetAvailabilityAsync(speciality, dateTime).ToListAsync();

        // Assert
        result.Should().NotBeNull();    
        result.Should().BeEmpty();
    }
}