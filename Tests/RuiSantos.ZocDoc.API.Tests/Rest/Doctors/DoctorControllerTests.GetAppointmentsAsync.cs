﻿using System.Net;
using FluentAssertions;
using RuiSantos.ZocDoc.Api.Contracts;
using RuiSantos.ZocDoc.Data.Dynamodb.Entities;

namespace RuiSantos.ZocDoc.API.Tests.Rest;

partial class DoctorControllerTests
{
    [Theory(DisplayName = "Should return a list of the doctor's appointments on a given date.")]
    [InlineData("DEF003", "2023-08-21")]
    public async Task GetAppointmentsAsync_ReturnsOk_WhenExistAppointment(string license, string dateTime)
    {
        // Act
        var response = await client.GetAsync($"/Doctor/{license}/Appointments/{dateTime}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert
        var content = await response.Content.As<DoctorAppointmentsContract[]>(output);
        content.Should().HaveCount(1);

        var element = content.First();
        element.Patient.SocialSecurityNumber.Should().Be("123-45-6789");
        element.Date.Should().Be(DateTime.Parse("2023-08-21 09:00:00"));
    }

    [Theory(DisplayName = "Should return the todays list of the doctor's appointments")]
    [InlineData("PED001")]
    public async Task GetAppointmentsAsync_ReturnsOk_WhenExistAppointmentToday(string license)
    {
        // Arrange
        var dateTime = DateTime.Today.AddHours(9).ToUniversalTime();

        var doctor = await DoctorDto.GetDoctorByLicenseAsync(context, license);
        var patient = await PatientDto.GetPatientBySocialSecurityNumberAsync(context, "123-45-6789");

        var appointment = new AppointmentsDto()
        {
            DoctorId = doctor!.Id,
            PatientId = patient!.Id,
            AppointmentDateTime = dateTime
        };       
        await context.SaveAsync(appointment);

        // Act
        var response = await client.GetAsync($"/Doctor/{license}/Appointments/");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert
        var content = await response.Content.As<DoctorAppointmentsContract[]>(output);
        content.Should().HaveCount(1);

        var element = content.First();
        element.Patient.SocialSecurityNumber.Should().Be(patient!.SocialSecurityNumber);
        element.Date.Should().Be(dateTime);

        // Teardown
        await context.DeleteAsync(appointment);
    }

    [Theory(DisplayName = "Should return empty if no records are found for the given doctor and date.")]
    [InlineData("XYZ002", null)]
    [InlineData("PED001", "2023-08-22")]
    [InlineData("DEF003", "2023-08-22")]
    public async Task GetAppointmentsAsync_ReturnsNoContent_WhenNotExistAppointment(string license, string? dateTime)
    {
        // Act & Assert
        var response = await client.GetAsync($"/Doctor/{license}/Appointments/{dateTime}");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}