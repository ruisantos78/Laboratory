using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using RuiSantos.ZocDoc.Core.Data;
using RuiSantos.ZocDoc.Core.Managers;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Core.Tests.Core;
using System.Linq.Expressions;

namespace RuiSantos.ZocDoc.Core.Tests;

public class AppointmentManagementTests
{
    private readonly Mock<IDataContext> mockDataContext = new Mock<IDataContext>();
    private readonly Mock<ILogger<AppointmentManagement>> mockLogger = new Mock<ILogger<AppointmentManagement>>();

    [Fact]
    public async Task CreateAppointmentAsync_WithValidInput_ShouldStoreAppointment()
    {
        // Arrange
        var socialNumber = "123-45-6789";
        var medicalLicence = "ABC123";
        var dateTime = DateTime.Parse("2022-01-01 08:00");

        var patient = ModelsFactory.CreatePatient(socialNumber);
        var doctor = ModelsFactory.CreateDoctor(medicalLicence);
        doctor.OfficeHours.Add(new OfficeHour(dateTime.DayOfWeek, new[] { dateTime.TimeOfDay }));

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

    [Fact]
    public async Task DeleteAppointmentAsync_WithValidInput_ShouldRemoveAppointment()
    {
        // Arrange
        var socialNumber = "123-45-6789";
        var medicalLicence = "ABC123";
        var dateTime = DateTime.Now;

        var patient = ModelsFactory.CreatePatient(socialNumber);
        var doctor = ModelsFactory.CreateDoctor(medicalLicence);
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
}