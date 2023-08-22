using System.Net;
using Amazon.DynamoDBv2.DataModel;
using FluentAssertions;
using Org.BouncyCastle.Asn1.Ocsp;
using RuiSantos.ZocDoc.Api.Contracts;
using RuiSantos.ZocDoc.API.Tests.Fixtures;
using RuiSantos.ZocDoc.Data.Dynamodb.Entities;
using Xunit.Abstractions;

using static RuiSantos.ZocDoc.Data.Dynamodb.Mappings.ClassMapConstants;

namespace RuiSantos.ZocDoc.API.Tests.Rest;

public class DoctorControllerTests: IClassFixture<ServiceFixture>
{
    private readonly IDynamoDBContext context;
    private readonly HttpClient client;
    private readonly ITestOutputHelper output;

    public DoctorControllerTests(ServiceFixture service, ITestOutputHelper output)
    {
        this.context = service.GetContext();
        this.client = service.GetClient();
        this.output = output;
    }

    [Theory(DisplayName = "Should returns the doctor's information.")]
    [InlineData("XYZ002")]
    [InlineData("DEF003")]
    [InlineData("PED001")]
    public async Task GetAsync_ReturnsOk_WhenLicenseIsValid(string license)
    {
        // Act
        var response = await client.GetAsync($"/Doctor/{license}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert
        var content = await response.Content.As<DoctorContract>(output);
        content.License.Should().Be(license);        
    }

    [Theory(DisplayName = "Should return empty if no records are found for the given license.")]
    [InlineData("ABC123")]
    public async Task GetAsync_ReturnNoContent_WhenLicenseNotExists(string license)
    {
        // Act & Assert
        var response = await client.GetAsync($"/Doctor/{license}");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

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
            AppointmentTime = dateTime
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

    [Fact(DisplayName = "Should returns a success message if the doctor is successfully created.")]
    public async Task PostAsync_ReturnsCreated_WhenDoctorIsCreated()
    {
        // Arrange
        var request = new DoctorContract
        {
            License = "ZZZ123",
            FirstName = "Joe",
            LastName = "Doe",
            Email = "joe.doe@email.net",
            ContactNumbers = new[] { "1-555-5555" },
            Specialties = new[] { "Cardiology" }
        };

        // Act
        var response = await client.PostAsync("/Doctor/", request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        // Assert
        var doctor = await context.FindAsync<DoctorDto>(DoctorLicenseIndexName, "ZZZ123");
        doctor.Should().NotBeNull();
        doctor!.FirstName.Should().Be("Joe");
        doctor!.LastName.Should().Be("Doe");
        doctor!.Email.Should().Be("joe.doe@email.net");
        doctor!.ContactNumbers.Should().HaveCount(1).And.ContainSingle("1-555-5555");
        
        var specialties = await context.QueryAsync<DoctorSpecialtyDto>(doctor.Id)
            .GetRemainingAsync();

        specialties.Should().NotBeNullOrEmpty();
        specialties.Should().HaveCount(1).And.ContainSingle(x => x.Specialty == "Cardiology");

        // Teardown
        await context.DeleteAsync(doctor);
    }

    [Theory(DisplayName = "Should returns a success message if the office hours are successfully set.")]
    [InlineData("CAR002", (int)DayOfWeek.Monday)]
    [InlineData("CAR002", (int)DayOfWeek.Wednesday)]
    [InlineData("CAR002", (int)DayOfWeek.Friday)]
    public async Task PutOfficeHoursAsync_ReturnsOk_WhenOfficeHoursAreUpdated(string license, int week)
    {
        // Arrange
        var officeHours = Enumerable.Range(9, 4)
            .SelectMany(i => new[] {
                TimeSpan.FromHours(i),
                TimeSpan.FromMinutes(i * 60 + 30) }
            );

        var request = officeHours.Select(x => x.ToString("g"))
            .ToArray();

        // Act
        var response = await client.PostAsync($"/Doctor/{license}/OfficeHours/{week}", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert
        var doctor = await context.FindAsync<DoctorDto>(DoctorLicenseIndexName, "CAR002");
        doctor.Should().NotBeNull();
        doctor!.Availability.Should().HaveCount(1);
        doctor.Availability.Should().ContainSingle(i => (int)i.Week == week && i.Hours.SequenceEqual(officeHours));

        // Teardown
        doctor.Availability.Clear();
        await context.SaveAsync(doctor);
    }
}