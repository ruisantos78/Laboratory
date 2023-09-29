using System.Net;
using FluentAssertions;
using RuiSantos.Labs.Api.Contracts;
using RuiSantos.Labs.Data.Dynamodb.Entities;

namespace RuiSantos.Labs.Api.Tests.Rest;

partial class DoctorControllerTests
{
    [Theory(DisplayName = "Should return a list of the doctor's appointments on a given date.")]
    [InlineData("8a6151c7-9122-4f1b-a1e7-85e981c17a14", "2023-08-21")]
    public async Task ShouldReturnListOfAppointmentsOnDate(string uuid, string dateTime)
    {
        // Act
        var response = await client.GetAsync($"/Doctor/{uuid}/Appointments/{dateTime}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert
        var content = await response.Content.GetModelAsync<DoctorAppointmentsContract[]>(output);
        content.Should().HaveCount(1);

        var element = content.First();
        element.Patient.SocialSecurityNumber.Should().Be("123-45-6789");
        element.Date.Should().Be(DateTime.Parse("2023-08-21 09:00:00"));
    }

    [Theory(DisplayName = "Should return the today's list of the doctor's appointments")]
    [InlineData("fa626a8e-03a5-4fe1-a910-b3803bed256c", "d4d94a6c-6a7f-4a98-90bb-6f2289c726d0")]
    public async Task ShouldReturnListOfAppointmentsToday(string doctorStringId, string patientStringId)
    {
        // Arrange
        var doctorId = Guid.Parse(doctorStringId);
        var patientId = Guid.Parse(patientStringId);
        var dateTime = DateTime.Today.AddHours(9).ToUniversalTime();

        var doctor = await context.LoadAsync<DoctorEntity>(doctorId);
        if (doctor is null) 
            Assert.Fail("Error Load Doctor");

        var patient = await context.LoadAsync<PatientEntity>(patientId);
        if (patient is null) 
            Assert.Fail("Error Load Patient");

        var appointment = await CreateTestAppointmentAsync(doctor.Id, patient.Id, dateTime);

        // Act
        var response = await client.GetAsync($"/Doctor/{doctorStringId}/Appointments/");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert
        var content = await response.Content.GetModelAsync<DoctorAppointmentsContract[]>(output);
        content.Should().HaveCount(1);

        var element = content.First();
        
        element.Patient.Should().BeEquivalentTo(new PatientContract {
            SocialSecurityNumber = patient.SocialSecurityNumber,
            Email = patient.Email,
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            ContactNumbers = patient.ContactNumbers
        });

        element.Date.Should().Be(dateTime);

        // Teardown
        await context.DeleteAsync(appointment);
    }

    [Theory(DisplayName = "Should return empty if no records are found for the given doctor and date.")]
    [InlineData("d6c9f315-0e35-4d5b-b25e-61a61c92d9c9", null)]
    [InlineData("fa626a8e-03a5-4fe1-a910-b3803bed256c", "2023-08-22")]
    [InlineData("8a6151c7-9122-4f1b-a1e7-85e981c17a14", "2023-08-22")]
    public async Task ShouldReturnEmptyForNonExistingAppointments(string uuid, string? dateTime)
    {
        // Act & Assert
        var response = await client.GetAsync($"/Doctor/{uuid}/Appointments/{dateTime}");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    // Helper method to create an appointment
    private async Task<AppointmentsEntity> CreateTestAppointmentAsync(Guid doctorId, Guid patientId, DateTime dateTime)
    {
        var entity = new AppointmentsEntity()
        {
            DoctorId = doctorId,
            PatientId = patientId,
            AppointmentDateTime = dateTime
        };

        await context.SaveAsync(entity);
        return entity;
    }
}