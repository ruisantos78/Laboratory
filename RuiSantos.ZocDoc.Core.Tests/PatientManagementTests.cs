using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using RuiSantos.ZocDoc.Core.Data;
using RuiSantos.ZocDoc.Core.Managers;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Core.Tests.Factories;
using System.Linq.Expressions;

namespace RuiSantos.ZocDoc.Core.Tests;

public class PatientManagementTests
{
    private readonly Mock<IDataContext> mockDataContext = new Mock<IDataContext>();
    private readonly Mock<ILogger<PatientManagement>> mockLogger = new Mock<ILogger<PatientManagement>>();

    [Fact]
    public async Task CreatePatientAsync_WithValidInput_SholdStorePatient()
    {
        // Arrange
        var args = PatientFactory.Create();
        var patient = PatientFactory.Empty();

        mockDataContext.Setup(m => m.StoreAsync(It.IsAny<Patient>()))
            .Callback<Patient>(value => patient = value);

        var manager = new PatientManagement(mockDataContext.Object, mockLogger.Object);

        // Act
        await manager.CreatePatientAsync(args.SocialSecurityNumber, args.Email, args.FirstName, args.LastName, args.ContactNumbers);

        // Assert
        mockDataContext.Verify(m => m.StoreAsync(It.IsAny<Patient>()), Times.Once);

        patient.Should().NotBeNull();
        patient.Id.Should().NotBe(args.Id).And.NotBeEmpty();
        patient.SocialSecurityNumber.Should().Be(args.SocialSecurityNumber);
        patient.Email.Should().Be(args.Email);
        patient.FirstName.Should().Be(args.FirstName);
        patient.LastName.Should().Be(args.LastName);
        patient.ContactNumbers.Should().BeEquivalentTo(args.ContactNumbers);
    }

    [Fact]
    public async Task GetPatientBySocialNumberAsync_WithValidInput_ReturnsExpectedResult()
    {
        // Arrange
        var socialNumber = "123-45-6789";

        mockDataContext.Setup(m => m.FindAsync(It.IsAny<Expression<Func<Patient, bool>>>()))
            .ReturnsAsync(PatientFactory.Create(socialNumber));

        var manager = new PatientManagement(mockDataContext.Object, mockLogger.Object);

        // Act
        var result = await manager.GetPatientBySocialNumberAsync(socialNumber);

        // Assert
        result.Should().NotBeNull();
        result?.Id.Should().NotBeEmpty();
        result?.SocialSecurityNumber.Should().Be(socialNumber);
    }

    [Fact]
    public async Task GetAppointmentsAsync_WithValidInput_ReturnsExpectedResult()
    {
        // Arrange
        var socialNumber = "555-00-0000";
        var dateTime = DateTime.Parse("2022-01-04 08:00");

        var appointment = new Appointment(dateTime);
        var patient = PatientFactory.Create(socialNumber).SetAppointments(appointment);
        var doctor = DoctorFactory.Create().SetAppointments(appointment);

        var appointments = AppointmentsFactory.Create(5);
        var doctors = appointments.Select(a => DoctorFactory.Create().SetAppointments(a)).ToList();
        doctors.Add(doctor);

        mockDataContext.Setup(m => m.FindAsync(It.IsAny<Expression<Func<Patient, bool>>>())).ReturnsAsync(patient);
        mockDataContext.Setup(m => m.QueryAsync(It.IsAny<Expression<Func<Doctor, bool>>>())).ReturnsAsync(doctors);

        var management = new PatientManagement(mockDataContext.Object, mockLogger.Object);

        // Act
        var result = await management.GetAppointmentsAsync(socialNumber).ToListAsync();

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().ContainSingle().Subject.doctor.Should().Be(doctor);
        result.Should().ContainSingle().Subject.date.Should().Be(dateTime);
    }
}
