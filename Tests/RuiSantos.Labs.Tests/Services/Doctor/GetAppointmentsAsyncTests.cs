using FluentAssertions;
using RuiSantos.Labs.Core.Models;
using RuiSantos.Labs.Core.Resources;
using RuiSantos.Labs.Core.Services.Exceptions;
using RuiSantos.Labs.Tests.Asserts.Builders;
using RuiSantos.Labs.Tests.Asserts.Services;

namespace RuiSantos.Labs.Tests.Services.Doctor;

/// <UserStory>
/// As a doctor, I want to be able to query all my appointments for a given date, with the patient's information allocated to each appointment.
/// </UserStory>
public class GetAppointmentsAsyncTests
{
    [Theory(DisplayName = "The doctor should be able to query all of his appointments for a given date.")]
    [InlineData(default)]
    [InlineData("2023-01-01")]
    public async Task GetAppointmentsAsync_WithSuccess(string? dateString)
    {
        // Arrange
        var dateTime = DateTime.TryParse(dateString, out var dateValue) ? dateValue.Date : DateTime.Today;
        var doctor = new DoctorBuilder().Build();

        var appointments = new PatientAppointmentBuilder()
            .AddAppointment(dateTime.AddHours(10))
            .AddAppointment(dateTime.AddHours(11))
            .AddAppointment(dateTime.AddHours(12))
            .Build();

        var asserts = new DoctorsServiceAsserts();
        asserts.OnFindAsyncReturns(doctor.Id, result: doctor);

        var date = DateOnly.FromDateTime(dateTime);
        asserts.OnGetPatientAppointmentsAsyncReturns(doctor, date, result: appointments);

        // Act
        var service = asserts.GetService();
        var result = await service.GetAppointmentsAsync(doctor.Id, dateTime);

        // Assert        
        result.Should().BeEquivalentTo(appointments);

        asserts.ShouldLogError(false);
    }

    [Fact(DisplayName = "An empty list should be returned when the doctor is not registred.")]
    public async Task GetAppointmentsAsync_WithUnregisteredDoctor_ShouldReturnEmptyList()
    {
        // Arrange
        var doctor = new DoctorBuilder().Build();

        var asserts = new DoctorsServiceAsserts();
        asserts.OnFindAsyncReturns(doctor.Id, result: default);

        // Act
        var service = asserts.GetService();
        var result = await service.GetAppointmentsAsync(doctor.Id, default);

        // Assert        
        result.Should().BeEmpty();

        asserts.ShouldLogError(false);
    }

    [Fact(DisplayName = "An empty list should be returned when the doctor has no appointments for the given date.")]
    public async Task GetAppointmentsAsync_WithNoAppointments_ShouldReturnEmptyList()
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.Today);
        var doctor = new DoctorBuilder().Build();

        var asserts = new DoctorsServiceAsserts();
        asserts.OnFindAsyncReturns(doctor.Id, result: doctor);
        asserts.OnGetPatientAppointmentsAsyncReturns(doctor, today, result: Array.Empty<PatientAppointment>());

        // Act
        var service = asserts.GetService();
        var result = await service.GetAppointmentsAsync(doctor.Id, default);

        // Assert        
        result.Should().BeEmpty();

        asserts.ShouldLogError(false);
    }

    [Fact(DisplayName = "A log should be written when an unhandled exception occurs.")]
    public async Task GetAppointmentsAsync_WhenFailsToGetPatientAppointments_ThenThrowsException_AndLogsError()
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.Today);
        var doctor = new DoctorBuilder().Build();

        var asserts = new DoctorsServiceAsserts();
        asserts.OnFindAsyncReturns(doctor.Id, result: doctor);
        asserts.WhenGetPatientAppointmentsAsyncThrows(doctor, today);

        // Act
        var service = asserts.GetService();
        await service.Awaiting(x => x.GetAppointmentsAsync(doctor.Id, default))
            .Should()
            .ThrowAsync<ServiceFailException>()
            .WithMessage(MessageResources.DoctorsGetAppointmentsFail);

        // Assert
        asserts.ShouldLogError();
    }

    [Fact(DisplayName = "A log should be written when an unhandled exception occurs.")]
    public async Task GetAppointmentsAsync_WhenFailsToFindDoctor_ThenThrowsException_AndLogsError()
    {
        // Arrange
        var doctor = new DoctorBuilder().Build();

        var asserts = new DoctorsServiceAsserts();
        asserts.WhenFindAsyncThrows(doctor.Id);

        // Act
        var service = asserts.GetService();
        await service.Awaiting(x => x.GetAppointmentsAsync(doctor.Id, default))
            .Should()
            .ThrowAsync<ServiceFailException>()
            .WithMessage(MessageResources.DoctorsGetAppointmentsFail);

        // Assert
        asserts.ShouldLogError();
    }
}