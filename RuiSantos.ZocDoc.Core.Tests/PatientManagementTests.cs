using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using RuiSantos.ZocDoc.Core.Adapters;
using RuiSantos.ZocDoc.Core.Data;
using RuiSantos.ZocDoc.Core.Managers;
using RuiSantos.ZocDoc.Core.Managers.Exceptions;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Core.Tests.Factories;
using System.Linq.Expressions;

namespace RuiSantos.ZocDoc.Core.Tests;

public class PatientManagementTests
{
    private readonly Mock<IDoctorAdapter> doctorAdapterMock = new();
    private readonly Mock<IPatientAdapter> patientAdapterMock = new();
    private readonly Mock<ILogger<PatientManagement>> loggerMock = new();

    [Fact]
    public async Task CreatePatientAsync_WithValidInput_SholdStorePatient()
    {
        // Arrange
        var args = PatientFactory.Create();
        var patient = PatientFactory.Empty();

        patientAdapterMock.Setup(m => m.StoreAsync(It.IsAny<Patient>()))
            .Callback<Patient>(value => patient = value);

        var manager = new PatientManagement(patientAdapterMock.Object, doctorAdapterMock.Object, loggerMock.Object);

        // Act
        await manager.CreatePatientAsync(args.SocialSecurityNumber, args.Email, args.FirstName, args.LastName, args.ContactNumbers);

        // Assert
        patientAdapterMock.Verify(m => m.StoreAsync(It.IsAny<Patient>()), Times.Once);

        patient.Should().NotBeNull();
        patient.Id.Should().NotBe(args.Id).And.NotBeEmpty();
        patient.SocialSecurityNumber.Should().Be(args.SocialSecurityNumber);
        patient.Email.Should().Be(args.Email);
        patient.FirstName.Should().Be(args.FirstName);
        patient.LastName.Should().Be(args.LastName);
        patient.ContactNumbers.Should().BeEquivalentTo(args.ContactNumbers);
    }

    [Fact]
    public async Task CreatePatientAsync_WithInvalidSocialNumber_SholdRaiseError()
    {
        // Arrange
        var args = PatientFactory.Create(socialNumber: "Invalid SSN");
        var patient = PatientFactory.Empty();

        patientAdapterMock.Setup(m => m.StoreAsync(It.IsAny<Patient>()))
            .Callback<Patient>(value => patient = value);

        var manager = new PatientManagement(patientAdapterMock.Object, doctorAdapterMock.Object, loggerMock.Object);

        // Act & Assert
        var failures = await manager.Invoking(async m => await m.CreatePatientAsync(args.SocialSecurityNumber, args.Email, args.FirstName, args.LastName, args.ContactNumbers))
            .Should().ThrowAsync<ValidationFailException>();

        failures.Which.Errors.Should().ContainSingle()
            .Subject.PropertyName.Should().Be(nameof(Patient.SocialSecurityNumber));

        patientAdapterMock.Verify(m => m.StoreAsync(It.IsAny<Patient>()), Times.Never);
    }

    [Fact]
    public async Task CreatePatientAsync_WithInvalidEmail_SholdRaiseError()
    {
        // Arrange
        var args = PatientFactory.Create(email: "Invalid Email");
        var patient = PatientFactory.Empty();

        patientAdapterMock.Setup(m => m.StoreAsync(It.IsAny<Patient>()))
            .Callback<Patient>(value => patient = value);

        var manager = new PatientManagement(patientAdapterMock.Object, doctorAdapterMock.Object, loggerMock.Object);

        // Act & Assert
        var failures = await manager.Invoking(async m => await m.CreatePatientAsync(args.SocialSecurityNumber, args.Email, args.FirstName, args.LastName, args.ContactNumbers))
            .Should().ThrowAsync<ValidationFailException>();

        failures.Which.Errors.Should().ContainSingle()
            .Subject.PropertyName.Should().Be(nameof(Patient.Email));

        patientAdapterMock.Verify(m => m.StoreAsync(It.IsAny<Patient>()), Times.Never);
    }

    [Fact]
    public async Task GetPatientBySocialNumberAsync_WithValidInput_ReturnsExpectedResult()
    {
        // Arrange
        var socialNumber = "100-00-0005";

        var patients = Enumerable.Range(1, 5).Select(i => PatientFactory.Create($"100-00-000{i}"));

        patientAdapterMock.Setup(m => m.FindAsync(socialNumber))
            .ReturnsAsync(patients.FirstOrDefault(p => p.SocialSecurityNumber == socialNumber));

        var manager = new PatientManagement(patientAdapterMock.Object, doctorAdapterMock.Object, loggerMock.Object);

        // Act
        var result = await manager.GetPatientBySocialNumberAsync(socialNumber);

        // Assert
        result.Should().NotBeNull();
        result?.Id.Should().NotBeEmpty();
        result?.SocialSecurityNumber.Should().Be(socialNumber);
    }

    [Fact]
    public async Task GetPatientBySocialNumberAsync_WithInvalidInput_ReturnsNullResult()
    {
        // Arrange
        var socialNumber = String.Empty;

        var patients = Enumerable.Range(1, 5).Select(i => PatientFactory.Create($"100-00-000{i}"));

        patientAdapterMock.Setup(m => m.FindAsync(socialNumber))
            .ReturnsAsync(patients.FirstOrDefault(p => p.SocialSecurityNumber == socialNumber));

        var manager = new PatientManagement(patientAdapterMock.Object, doctorAdapterMock.Object, loggerMock.Object);

        // Act
        var result = await manager.GetPatientBySocialNumberAsync(socialNumber);

        // Assert
        result.Should().BeNull();
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

        patientAdapterMock.Setup(m => m.FindAsync(socialNumber)).ReturnsAsync(patient);
        doctorAdapterMock.Setup(m => m.FindAllWithAppointmentsAsync(It.IsAny<List<Appointment>>())).ReturnsAsync(new List<Doctor>() { doctor });

        var manager = new PatientManagement(patientAdapterMock.Object, doctorAdapterMock.Object, loggerMock.Object);

        // Act
        var result = await manager.GetAppointmentsAsync(socialNumber).ToListAsync();

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().ContainSingle().Subject.Doctor.Should().Be(doctor);
        result.Should().ContainSingle().Subject.Date.Should().Be(dateTime);
    }
}
