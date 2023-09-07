using FluentAssertions;
using RuiSantos.Labs.Core.Models;
using RuiSantos.Labs.Core.Resources;
using RuiSantos.Labs.Core.Services.Exceptions;
using RuiSantos.Labs.Tests.Asserts;
using RuiSantos.Labs.Tests.Asserts.Builders;

namespace RuiSantos.Labs.Tests;

/// <UserStory>
/// As a doctor, I want to be able to query all my appointments for a given date, with the patient's information allocated to each appointment.
/// </UserStory>
public class DoctorAppointmentsReport
{
    [Theory(DisplayName = "The doctor should be able to query all of his appointments for a given date.")]
    [InlineData("ABC123", default)]
    [InlineData("ABC123", "2023-01-01")]
    public async void GetAppointmentsAsync_WithSuccess(string license, string? date)
    {
        // Arrange
        DateTime? dateTime = date is null ? null : DateTime.Parse(date);
        var day = DateOnly.FromDateTime(dateTime ?? DateTime.Today);

        var doctor =  new DoctorBuilder()
            .With(license)            
            .Build();

        var appointements = new PatientAppointmentBuilder()
            .AddAppointment(day.ToDateTime(new TimeOnly(10, 0, 0)), "111-11-1111")
            .AddAppointment(day.ToDateTime(new TimeOnly(11, 30, 0)), "222-22-2222")
            .AddAppointment(day.ToDateTime(new TimeOnly(13, 0, 0)), "333-33-3333")            
            .Build();

        var asserts = new DoctorsAsserts();
        asserts.ReturnsOnFindAsync(license, doctor);
        asserts.ReturnsOnGetPatientAppointmentsAsync(doctor, day, appointements);

        // Act
        var service = asserts.GetService();
        var result = await service.GetAppointmentsAsync(license, dateTime);

        // Assert        
        result.Should().BeEquivalentTo(appointements);        

        asserts.ShouldNotLogError();    
    }

    [Fact(DisplayName = "An empty list should be returned when the doctor is not registred.")]
    public async Task GetAppointmentsAsync_WithUnregisteredDoctor_ShouldReturnEmptyList()
    {
        // Arrange
        var license = "ABC123";
        var today = DateOnly.FromDateTime(DateTime.Today);

        var asserts = new DoctorsAsserts();
        asserts.ReturnsOnFindAsync(license, null);

        // Act
        var service = asserts.GetService();
        var result = await service.GetAppointmentsAsync(license, default);

        // Assert        
        result.Should().BeEmpty();        

        asserts.ShouldNotLogError();    
    }

    [Fact(DisplayName = "An empty list should be returned when the doctor has no appointments for the given date.")]
    public async Task GetAppointmentsAsync_WithNoAppointments_ShouldReturnEmptyList()
    {
        // Arrange
        var license = "ABC123";
        var today = DateOnly.FromDateTime(DateTime.Today);

        var doctor =  new DoctorBuilder()
            .With(license)            
            .Build();

        var asserts = new DoctorsAsserts();
        asserts.ReturnsOnFindAsync(license, doctor);
        asserts.ReturnsOnGetPatientAppointmentsAsync(doctor, today, Array.Empty<PatientAppointment>());

        // Act
        var service = asserts.GetService();
        var result = await service.GetAppointmentsAsync(license, default);

        // Assert        
        result.Should().BeEmpty();        

        asserts.ShouldNotLogError();    
    }

    [Fact(DisplayName = "A log should be written when an unhandled exception occurs.")]
    public async Task GetAppointmentsAsync_WhenFailsToGetPatientAppointments_ThenThrowsException_AndLogsError()
    {
        // Arrange
        var license = "ABC123";
        var today = DateOnly.FromDateTime(DateTime.Today);

        var doctor =  new DoctorBuilder()
            .With(license)            
            .Build();

        var asserts = new DoctorsAsserts();
        asserts.ReturnsOnFindAsync(license, doctor);
        asserts.ThrowsOnGetPatientAppointmentsAsync(doctor, today);

        // Act
        var service = asserts.GetService();
        await service.Awaiting(x => x.GetAppointmentsAsync(license, default))
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
        var license = "ABC123";
        var today = DateOnly.FromDateTime(DateTime.Today);

        var doctor =  new DoctorBuilder()
            .With(license)            
            .Build();

        var asserts = new DoctorsAsserts();
        asserts.ThrowOnFindAsync(license);

        // Act
        var service = asserts.GetService();
        await service.Awaiting(x => x.GetAppointmentsAsync(license, default))
            .Should()
            .ThrowAsync<ServiceFailException>()
            .WithMessage(MessageResources.DoctorsGetAppointmentsFail);

        // Assert
        asserts.ShouldLogError();    
    }
}