using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using RuiSantos.ZocDoc.Core.Data;
using RuiSantos.ZocDoc.Core.Managers;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Core.Tests.Factories;
using System.Linq.Expressions;

namespace RuiSantos.ZocDoc.Core.Tests;

/// <summary>
/// Tests for <see cref="AppointmentManagement"/>
/// </summary>
public class AppointmentManagementTests
{
    /// <summary>
    /// Mocks the data context.
    /// </summary>
    private readonly Mock<IDataContext> mockDataContext = new();

    /// <summary>
    /// Mocks the logger.
    /// </summary>
    private readonly Mock<ILogger<AppointmentManagement>> mockLogger = new();

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

        mockDataContext.Setup(m => m.FindAsync(It.IsAny<Expression<Func<Patient, bool>>>())).ReturnsAsync(patient);
        mockDataContext.Setup(m => m.StoreAsync(patient)).Verifiable();

        mockDataContext.Setup(m => m.FindAsync(It.IsAny<Expression<Func<Doctor, bool>>>())).ReturnsAsync(doctor);
        mockDataContext.Setup(m => m.StoreAsync(doctor)).Verifiable();

        var appointmentManagement = new AppointmentManagement(mockDataContext.Object, mockLogger.Object);

        // Act
        await appointmentManagement.CreateAppointmentAsync(socialNumber, medicalLicence, dateTime);

        // Assert
        patient.Appointments.Should().ContainSingle(appointment => appointment.GetDateTime() == dateTime);
        doctor.Appointments.Should().ContainSingle(appointment => appointment.GetDateTime() == dateTime);

        mockDataContext.Verify(m => m.StoreAsync(patient), Times.Once);
        mockDataContext.Verify(m => m.StoreAsync(doctor), Times.Once);
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

        mockDataContext.Setup(m => m.FindAsync(It.IsAny<Expression<Func<Patient, bool>>>())).ReturnsAsync(patient);
        mockDataContext.Setup(m => m.StoreAsync(patient)).Verifiable();

        mockDataContext.Setup(m => m.FindAsync(It.IsAny<Expression<Func<Doctor, bool>>>())).ReturnsAsync(doctor);
        mockDataContext.Setup(m => m.StoreAsync(doctor)).Verifiable();

        var appointmentManagement = new AppointmentManagement(mockDataContext.Object, mockLogger.Object);

        // Act
        await appointmentManagement.DeleteAppointmentAsync(socialNumber, medicalLicence, dateTime);

        // Assert
        patient.Appointments.Should().BeEmpty();
        doctor.Appointments.Should().BeEmpty();

        mockDataContext.Verify(m => m.StoreAsync(patient), Times.AtMostOnce);
        mockDataContext.Verify(m => m.StoreAsync(doctor), Times.AtMostOnce);
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

        var doctors = new[]
        {
            DoctorFactory.Create("001").SetSpecialties(speciality).SetOfficeHours(officeHours).SetAppointments(appointments),
            DoctorFactory.Create("002").SetSpecialties("another speciality").SetOfficeHours(officeHours).SetAppointments(appointments)
        };

        mockDataContext.Setup(m => m.QueryAsync(It.IsAny<Expression<Func<Doctor, bool>>>()))
            .ReturnsAsync((Expression<Func<Doctor, bool>> expression) => doctors.Where(expression.Compile()).ToList());

        var appointmentManagement = new AppointmentManagement(mockDataContext.Object, mockLogger.Object);

        // Act
        var result = await appointmentManagement.GetAvailabilityAsync(speciality, dateTime).ToListAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
        result.Should().ContainSingle().Which.doctor.License.Should().Be("001");
        result.Should().ContainSingle().Which.schedule.Should().HaveCount(3);
        result.Should().ContainSingle().Which.schedule.Should().NotContain(dateTime);

        mockDataContext.Verify();
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

        mockDataContext.Setup(m => m.QueryAsync(It.IsAny<Expression<Func<Doctor, bool>>>()))
            .ReturnsAsync(new List<Doctor>());

        var management = new AppointmentManagement(mockDataContext.Object, mockLogger.Object);       
        
        // Act
        var result = await management.GetAvailabilityAsync(speciality, dateTime).ToListAsync();

        // Assert
        result.Should().NotBeNull();    
        result.Should().BeEmpty();
    }
}