using Microsoft.Extensions.Logging;
using RuiSantos.ZocDoc.Core.Adapters;
using RuiSantos.ZocDoc.Core.Managers.Exceptions;
using RuiSantos.ZocDoc.Core.Tests.Adapters;
using RuiSantos.ZocDoc.Core.Tests.Factories;

namespace RuiSantos.ZocDoc.Core.Tests;

public class PatientManagementTests
{
    private readonly DoctorAdapterMock doctorAdapterMock = new();
    private readonly PatientAdapterMock patientAdapterMock = new();
    private readonly Mock<ILogger<PatientManagement>> loggerMock = new();

    private IPatientManagement Manager => new PatientManagement(patientAdapterMock.Object, doctorAdapterMock.Object, loggerMock.Object);

    [Fact]
    public async Task CreatePatientAsync_WithValidInput_SholdStorePatient()
    {
        // Arrange
        var patients = new List<Patient>();
        patientAdapterMock.SetStoreAsyncCallback(patients.Add);

        // Act
        var patient = PatientBuilder.Dummy()
            .Build();

        await Manager.CreatePatientAsync(
            patient.SocialSecurityNumber,
            patient.Email,
            patient.FirstName,
            patient.LastName,
            patient.ContactNumbers);

        // Assert
        patients.Should().NotBeNullOrEmpty().And.HaveCount(1);
        patients.Should().ContainSingle(p =>
            p.SocialSecurityNumber == patient.SocialSecurityNumber &&
            p.Email == patient.Email &&
            p.FirstName == patient.FirstName &&
            p.LastName == patient.LastName);
    }

    [Fact]
    public async Task CreatePatientAsync_WithInvalidSocialNumber_SholdRaiseError()
    {
        // Arrange
        var patients = new List<Patient>();
        patientAdapterMock.SetStoreAsyncCallback(patients.Add);

        // Act & Assert
        var patient = PatientBuilder.Dummy(socialNumber: "Invalid SSN")
            .Build();

        var failures = await Manager.Awaiting(m => m.CreatePatientAsync(
            patient.SocialSecurityNumber,
            patient.Email,
            patient.FirstName,
            patient.LastName,
            patient.ContactNumbers))
            .Should().ThrowAsync<ValidationFailException>();

        failures.Which.Errors.Should()
            .ContainSingle(e => e.PropertyName == nameof(Patient.SocialSecurityNumber));

        patientAdapterMock.ShouldNotStoreAsync();
    }

    [Fact]
    public async Task CreatePatientAsync_WithInvalidEmail_SholdRaiseError()
    {
        // Arrange
        var patients = new List<Patient>();
        patientAdapterMock.SetStoreAsyncCallback(patients.Add);

        // Act & Assert
        var patient = PatientBuilder.Dummy(email: "Invalid Email")
            .Build();

        var failures = await Manager.Awaiting(m => m.CreatePatientAsync(
            patient.SocialSecurityNumber,
            patient.Email,
            patient.FirstName,
            patient.LastName,
            patient.ContactNumbers))
            .Should().ThrowAsync<ValidationFailException>();

        failures.Which.Errors.Should()
            .ContainSingle(e => e.PropertyName == nameof(Patient.Email));

        patientAdapterMock.ShouldNotStoreAsync();
    }

    [Fact]
    public async Task GetPatientBySocialNumberAsync_WithValidInput_ReturnsExpectedResult()
    {
        // Arrange
        var patients = Enumerable.Range(1, 5)
            .Select(i => PatientBuilder.Dummy(socialNumber: $"100-00-000{i}").Build());

        patientAdapterMock.SetFindAsyncReturns(socialNumber =>
            patients.FirstOrDefault(p => p.SocialSecurityNumber == socialNumber));

        // Act
        var result = await Manager.GetPatientBySocialNumberAsync("100-00-0005");

        // Assert
        result.Should().BeOfType<Patient>()
            .Which.SocialSecurityNumber.Should().Be("100-00-0005");
    }

    [Fact]
    public async Task GetPatientBySocialNumberAsync_WithInvalidInput_ReturnsNullResult()
    {
        // Arrange
        var patients = Enumerable.Range(1, 5)
            .Select(i => PatientBuilder.Dummy(socialNumber: $"100-00-000{i}").Build());

        patientAdapterMock.SetFindAsyncReturns(socialNumber =>
            patients.FirstOrDefault(p => p.SocialSecurityNumber == socialNumber));

        // Act
        var result = await Manager.GetPatientBySocialNumberAsync("999-99-9999");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAppointmentsAsync_WithValidInput_ReturnsExpectedResult()
    {
        // Arrange
        var dateTime = DateTime.Parse("2022-01-04 08:00");
        var appointments = Enumerable.Range(0, 5).Select(hour => new Appointment(dateTime.AddHours(hour)))
            .ToArray();

        patientAdapterMock.SetFindAsyncReturns(socialNumber => PatientBuilder.Dummy(socialNumber)
            .AddAppointments(appointments.First())
            .Build());

        doctorAdapterMock.SetFindAllWithAppointmentsAsyncReturns(DoctorBuilder.Dummy("ABC123")
            .AddAppointments(appointments.ToArray())
            .BuildList());

        // Act
        var result = await Manager.GetAppointmentsAsync("555-00-0000").ToListAsync();

        // Assert
        result.Should().NotBeNullOrEmpty()
            .And.ContainSingle(da => da.Doctor.License == "ABC123" && da.Date == dateTime);            
    }
}
