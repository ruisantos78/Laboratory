using Microsoft.Extensions.Logging;
using RuiSantos.ZocDoc.Core.Tests.Adapters;
using RuiSantos.ZocDoc.Core.Tests.Factories;

namespace RuiSantos.ZocDoc.Core.Tests;

/// <summary>
/// Tests for <see cref="Managers.AppointmentManagement"/>
/// </summary>
public class AppointmentManagementTests
{
    private readonly DoctorAdapterMock doctorAdapterMock = new();
    private readonly PatientAdapterMock patientAdapterMock = new();
    private readonly Mock<ILogger<AppointmentManagement>> loggerMock = new();

    private IAppointmentManagement Management =>
        new AppointmentManagement(doctorAdapterMock.Object, patientAdapterMock.Object, loggerMock.Object);

    /// <summary>
    /// Tests that <see cref="AppointmentManagement.CreateAppointmentAsync(string, string, DateTime)"/>
    /// store an appointment for a valid patient and doctor. 
    /// </summary>
    [Fact]
    public async Task CreateAppointmentAsync_WithValidInput_ShouldStoreAppointment()
    {
        // Arrange
        patientAdapterMock.SetFindAsyncReturns(socialNumber => PatientBuilder.Dummy(socialNumber)
            .Build());

        doctorAdapterMock.SetFindAsyncReturns(license => DoctorBuilder.Dummy(license)
            .AddOfficeHours(DayOfWeek.Saturday, TimeSpan.FromHours(8))
            .Build());

        // Act
        var dateTime = DateTime.Parse("2022-01-01 08:00");
        await Management.CreateAppointmentAsync("123-45-6789", "ABC123", dateTime);

        // Assert
        doctorAdapterMock.ShouldStoreAsync(doctor => doctor.Appointments.Any(app => app.GetDateTime() == dateTime));
        patientAdapterMock.ShouldStoreAsync(patient =>
            patient.Appointments.Any(app => app.GetDateTime() == dateTime));
    }

    /// <summary>
    /// Tests that <see cref="AppointmentManagement.CreateAppointmentAsync(string, string, DateTime)"/>
    /// remove an appointment for a valid patient and doctor.
    /// </summary>
    [Fact]
    public async Task DeleteAppointmentAsync_WithValidInput_ShouldRemoveAppointment()
    {
        // Arrange
        var dateTime = DateTime.Parse("2022-01-01 08:00");

        patientAdapterMock.SetFindAsyncReturns(socialNumber => PatientBuilder.Dummy(socialNumber)
            .AddAppointments(dateTime)
            .Build());

        doctorAdapterMock.SetFindAsyncReturns(license => DoctorBuilder.Dummy(license)
            .AddOfficeHours(dateTime.DayOfWeek, dateTime.TimeOfDay)
            .AddAppointments(dateTime)
            .Build());

        // Act
        await Management.DeleteAppointmentAsync("123-45-6789", "ABC123", dateTime);

        // Assert
        doctorAdapterMock.ShouldStoreAsync(doctor => !doctor.Appointments.Any(),
            Times.AtMostOnce);

        patientAdapterMock.ShouldStoreAsync(patient => !patient.Appointments.Any(),
            Times.AtMostOnce);
    }

    /// <summary>
    /// Tests that <see cref="AppointmentManagement.GetAvailabilityAsync(string, DateTime)"/>
    /// returns the availability for a valid doctor.
    /// </summary>
    [Fact]
    public async Task GetAvailabilityAsync_WithValidInput_ReturnsExpectedResult()
    {
        // Arrange
        var dateTime = DateTime.Parse("2022-01-03 09:00");

        var hours = Enumerable.Range(8, 4).Select(hour => TimeSpan.FromHours(hour)).ToArray();
        doctorAdapterMock.SetFindBySpecialtyWithAvailabilityAsyncReturns((speciality, _) => DoctorBuilder.Dummy()
            .AddSpecialties(speciality)
            .AddOfficeHours(DayOfWeek.Monday, hours)
            .AddOfficeHours(DayOfWeek.Tuesday, hours)
            .AddAppointments(dateTime)
            .BuildList());

        // Act       
        var result = await Management.GetAvailabilityAsync("Cardiology", dateTime).ToListAsync();

        // Assert
        result.Should().NotBeNullOrEmpty().And
            .ContainSingle(ds => ds.Schedule.Count == 3 && !ds.Schedule.Contains(dateTime));
    }

    /// <summary>
    /// This test is to ensure that when there are no doctors with the given speciality,
    /// the result is empty.
    /// </summary>
    [Fact]
    public async Task GetAvailabilityAsync_WithInvalidInput_ReturnsEmptyResult()
    {
        // Arrange
        doctorAdapterMock.SetFindBySpecialtyWithAvailabilityAsyncReturns(new List<Doctor>());

        // Act
        var dateTime = DateTime.Parse("2022-01-03 09:00");
        var result = await Management.GetAvailabilityAsync("cardiology", dateTime).ToListAsync();

        // Assert
        result.Should().NotBeNull().And.BeEmpty();
    }
}