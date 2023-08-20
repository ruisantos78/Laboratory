using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RuiSantos.ZocDoc.Core.Cache;
using RuiSantos.ZocDoc.Core.Tests.Repositories;
using RuiSantos.ZocDoc.Core.Tests.Builders;

namespace RuiSantos.ZocDoc.Core.Tests;

public class DoctorServiceTests
{
    private readonly DoctorRepositoryMock doctorAdapterMock = new();
    private readonly PatientRepositoryMock patientAdapterMock = new();
    private readonly Mock<IRepositoryCache> domainContextMock = new();
    private readonly Mock<ILogger<DoctorService>> loggerMock = new();

    private IDoctorService Management => new DoctorService(domainContextMock.Object, doctorAdapterMock.Object, patientAdapterMock.Object, loggerMock.Object);

    [Fact]
    public async Task CreateDoctorAsync_WithValidInput_ShouldStoreDoctor()
    {
        // Arrange
        var doctors = new List<Doctor>();
        doctorAdapterMock.SetStoreAsyncCallback(doctors.Add);

        domainContextMock.Setup(m => m.GetMedicalSpecialtiesAsync())
            .ReturnsAsync(SpecialtiesBuilder.With("Neurology").Build());

        // Act
        var doctor = DoctorBuilder.Dummy(specialty: "Neurology")
            .Build();

        await Management.CreateDoctorAsync(doctor.License, doctor.Email, doctor.FirstName,
            doctor.LastName, doctor.ContactNumbers, doctor.Specialties);

        // Assert
        doctorAdapterMock.ShouldStoreAsync();
        doctors.Should().NotBeNullOrEmpty()
            .And.ContainSingle(d =>
                d.License == doctor.License &&
                d.Email == doctor.Email &&
                d.FirstName == doctor.FirstName &&
                d.LastName == doctor.LastName)
            .Which.Specialties.Should().BeEquivalentTo(doctor.Specialties);
    }

    [Fact]
    public async Task SetOfficeHoursAsync_WithValidInput_ShouldUpdateDoctor()
    {
        // Arrange
        var doctors = new List<Doctor>();
        doctorAdapterMock.SetStoreAsyncCallback(doctors.Add);
        doctorAdapterMock.SetFindAsyncReturns(license => DoctorBuilder.Dummy(license).Build());

        // Act
        var hours = Enumerable.Range(8, 4).SelectMany(hour => new List<TimeSpan> {
            new(hour, 0, 0),
            new(hour, 30, 0)
        });

        await Management.SetOfficeHoursAsync("ABC123", DayOfWeek.Monday, hours);

        // Assert
        doctorAdapterMock.ShouldStoreAsync();

        doctors.Should().NotBeNullOrEmpty()
            .And.ContainSingle(doctor => doctor.License == "ABC123")
            .Which.OfficeHours.Should().ContainSingle(oh => oh.Week == DayOfWeek.Monday)
            .Which.Hours.Should().BeSameAs(hours);
    }

    [Fact]
    public async Task GetDoctorByLicenseAsync_WithValidInput_ReturnsExpectedResult()
    {
        // Arrange
        doctorAdapterMock.SetFindAsyncReturns(license => DoctorBuilder.Dummy(license).Build());

        // Act
        var result = await Management.GetDoctorByLicenseAsync("ABC123");

        // Assert
        result.Should().BeOfType<Doctor>()
            .Subject.License.Should().Be("ABC123");
    }

    [Fact]
    public async Task GetAppointmentsAsync_WithValidInput_ReturnsExpectedResult()
    {
        // Arrange
        var dateTime = DateTime.Parse("2022-01-04 08:00");

        doctorAdapterMock.SetFindAsyncReturns(license => DoctorBuilder.Dummy(license)
            .AddAppointments(Enumerable.Range(0, 5)
                .Select(hour => dateTime.AddHours(hour))
                .ToArray())
            .Build());

        patientAdapterMock.SetFindAllWithAppointmentsAsyncReturns(appointments =>
        {
            var i = 0;
            return appointments.Select(
                app => PatientBuilder.Dummy($"000-{i++:00}-0000").AddAppointments(app).Build())
            .ToList();
        });

        // Act
        var result = await Management.GetAppointmentsAsync("ABC123", dateTime).ToListAsync();

        // Assert
        result.Should().NotBeNullOrEmpty().And.HaveCount(5);

        result.Should().AllSatisfy(pa =>
            pa.Patient.SocialSecurityNumber.Should().MatchRegex("000-0[0-4]-0000"));
    }
}
