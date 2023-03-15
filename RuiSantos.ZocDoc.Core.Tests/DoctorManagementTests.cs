using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using RuiSantos.ZocDoc.Core.Adapters;
using RuiSantos.ZocDoc.Core.Data;
using RuiSantos.ZocDoc.Core.Managers;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Core.Tests.Factories;

namespace RuiSantos.ZocDoc.Core.Tests;

public class DoctorManagementTests
{
    private readonly Mock<IDoctorAdapter> doctorAdapterMock = new();
    private readonly Mock<IPatientAdapter> patientAdapterMock = new();
    private readonly Mock<IDomainContext> domainContextMock = new();
    private readonly Mock<ILogger<DoctorManagement>> loggerMock = new();

    [Fact]
    public async Task CreateDoctorAsync_WithValidInput_ShouldStoreDoctor()
    {
        // Arrange
        var specialty = "Neurology";
        var args = DoctorFactory.Create().SetSpecialties(specialty);

        var doctor = DoctorFactory.Empty();

        domainContextMock.Setup(m => m.GetMedicalSpecialtiesAsync()).ReturnsAsync(SpecialtyFactory.Create(specialty));
        doctorAdapterMock.Setup(m => m.StoreAsync(It.IsAny<Doctor>())).Callback<Doctor>(value => doctor = value);

        var management = new DoctorManagement(domainContextMock.Object, doctorAdapterMock.Object, patientAdapterMock.Object, loggerMock.Object);

        // Act
        await management.CreateDoctorAsync(args.License, args.Email, args.FirstName, args.LastName, args.ContactNumbers, args.Specialities);

        // Assert
        doctorAdapterMock.Verify(m => m.StoreAsync(It.IsAny<Doctor>()), Times.Once);

        doctor.Should().NotBeNull();
        doctor.Id.Should().NotBe(args.Id).And.NotBeEmpty();
        doctor.License.Should().Be(args.License);
        doctor.Email.Should().Be(args.Email);
        doctor.FirstName.Should().Be(args.FirstName);
        doctor.LastName.Should().Be(args.LastName);
        doctor.ContactNumbers.Should().BeEquivalentTo(args.ContactNumbers);
        doctor.Specialities.Should().BeEquivalentTo(args.Specialities);
    }

    [Fact]
    public async Task SetOfficeHoursAsync_WithValidInput_ShouldUpdateDoctor()
    {
        // Arrange
        var license = "ABC123";
        var dayOfWeek = DayOfWeek.Monday;
        var hours = new[] {
            new TimeSpan(8,0,0),
            new TimeSpan(8,3,0),
            new TimeSpan(9,0,0),
            new TimeSpan(9,3,0)
        };

        var doctor = DoctorFactory.Empty();

        doctorAdapterMock.Setup(m => m.FindAsync(license)).ReturnsAsync(DoctorFactory.Create(license));
        doctorAdapterMock.Setup(m => m.StoreAsync(It.IsAny<Doctor>())).Callback<Doctor>(value => doctor = value);

        var management = new DoctorManagement(domainContextMock.Object, doctorAdapterMock.Object, patientAdapterMock.Object, loggerMock.Object);

        // Act
        await management.SetOfficeHoursAsync(license, dayOfWeek, hours);

        // Assert
        doctorAdapterMock.Verify(m => m.StoreAsync(It.IsAny<Doctor>()), Times.Once);

        doctor.Should().NotBeNull();
        doctor.Id.Should().NotBeEmpty();
        doctor.License.Should().Be(license);
        doctor.OfficeHours.Should().NotBeNullOrEmpty();
        doctor.OfficeHours.Should().ContainSingle().Subject.Week.Should().Be(dayOfWeek);
        doctor.OfficeHours.Should().ContainSingle().Subject.Hours.Should().BeSameAs(hours);
    }

    [Fact]
    public async Task GetDoctorByLicenseAsync_WithValidInput_ReturnsExpectedResult()
    {
        // Arrange
        var license = "ABC123";

        doctorAdapterMock.Setup(m => m.FindAsync(license)).ReturnsAsync(DoctorFactory.Create(license));

        var management = new DoctorManagement(domainContextMock.Object, doctorAdapterMock.Object, patientAdapterMock.Object, loggerMock.Object);

        // Act
        var result = await management.GetDoctorByLicenseAsync(license);

        // Assert
        result.Should().NotBeNull();
        result?.Id.Should().NotBeEmpty();
        result?.License.Should().Be(license);
    }

    [Fact]
    public async Task GetAppointmentsAsync_WithValidInput_ReturnsExpectedResult()
    {
        // Arrange
        var license = "ABC123";
        var dateTime = DateTime.Parse("2022-01-04 08:00");

        var appointment = new Appointment(dateTime);
        var doctor = DoctorFactory.Create(license).SetAppointments(appointment);
        var patient = PatientFactory.Create().SetAppointments(appointment);

        var appointments = AppointmentsFactory.Create(5);
        var patients = appointments.Select(a => PatientFactory.Create().SetAppointments(a)).ToList();
        patients.Add(patient);

        doctorAdapterMock.Setup(m => m.FindAsync(doctor.License)).ReturnsAsync(doctor);
        patientAdapterMock.Setup(m => m.FindAllWithAppointmentsAsync(It.IsAny<List<Appointment>>())).ReturnsAsync(new List<Patient>() { patient });

        var management = new DoctorManagement(domainContextMock.Object, doctorAdapterMock.Object, patientAdapterMock.Object, loggerMock.Object);

        // Act
        var result = await management.GetAppointmentsAsync(license, dateTime).ToListAsync();

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().ContainSingle().Subject.Patient.Should().Be(patient);
        result.Should().ContainSingle().Subject.Date.Should().Be(dateTime);
    }
}
