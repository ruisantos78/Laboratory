using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Data.Dynamodb.Repositories;
using RuiSantos.ZocDoc.Data.Dynamodb.Tests.Fixtures;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Tests;

[Collection("Doctor Management")]
public class DoctorsTests: IClassFixture<DatabaseFixture>
{
    private readonly DoctorRepository doctorRepository;
    
    public DoctorsTests(DatabaseFixture database)
    {
        this.doctorRepository = new(database.Client);
    }

    [Fact(DisplayName = "Should create a new doctor.")]
    public async Task StoreAsync_ShouldCreateANewDoctor()
    {
        // Arrange
        var doctor = new Doctor()
        {
            License = "XYZ001",
            FirstName = "Jhon",
            LastName = "Doe",
            Email = "doe.jhon@doctor.com",
            ContactNumbers = new() { "5555555555" },
            Specialties = new() { "Cardiology", "Pediatrics" }
        };

        // Act
        await this.doctorRepository.StoreAsync(doctor);

        // Assert
        var result = await this.doctorRepository.FindAsync(doctor.License);
        result.Should().NotBeNull();
        result!.License.Should().Be(doctor.License);
        result.Specialties.Should().BeEquivalentTo(doctor.Specialties);
        result.ContactNumbers.Should().BeEquivalentTo(doctor.ContactNumbers);
        result.Appointments.Should().BeEmpty();
        result.OfficeHours.Should().BeEmpty();
    }

    [Fact(DisplayName = "Should set the office hours to a doctor")]
    public async Task StoreAsync_ShouldSetTheOfficeHoursToADoctor()
    {
        // Arrange
        var doctor = await this.doctorRepository.FindAsync("PED001");
        doctor!.OfficeHours.Clear();
        doctor.OfficeHours.Add(new()
        {
            Week = DayOfWeek.Tuesday,
            Hours = new[] { TimeSpan.FromHours(13), TimeSpan.FromHours(15) }
        });

        // Act
        await this.doctorRepository.StoreAsync(doctor);

        // Assert
        var result = await this.doctorRepository.FindAsync(doctor.License);
        result.Should().NotBeNull();
        result!.License.Should().Be(doctor.License);
        result.OfficeHours.Should().BeEquivalentTo(doctor.OfficeHours);
    }

    [Fact(DisplayName = "Should find doctor by license.")]
    public async Task FindAsync_ShouldFindDoctor_WithLicenseNumber()
    {
        // Arrange
        const string licenseNumber = "XYZ002";
        const string expectedId = "d6c9f315-0e35-4d5b-b25e-61a61c92d9c9";

        // Act
        var doctor = await this.doctorRepository.FindAsync(licenseNumber);

        // Assert
        doctor.Should().NotBeNull();
        doctor!.License.Should().Be(licenseNumber);
        doctor!.Id.Should().Be(expectedId);
    }

    [Fact(DisplayName = "Should find doctors by specialties.")]
    public async Task FindBySpecialityAsync_ShouldFindDoctor_WithSpecialty()
    {
        // Arrange
        const string specialty = "Dermatology";
        var expectedIds = new[] { "d6c9f315-0e35-4d5b-b25e-61a61c92d9c9", "8a6151c7-9122-4f1b-a1e7-85e981c17a14" };

        // Act
        var doctors = await this.doctorRepository.FindBySpecialityAsync(specialty);

        // Assert
        doctors.Should().NotBeNull().And.HaveCount(2);
        doctors.Should().OnlyContain(d => expectedIds.Contains(d.Id.ToString()));
    }

    [Fact(DisplayName = "Should find doctors by specialties with availability on a date.")]
    public async Task FindBySpecialtyWithAvailabilityAsync_ShouldFindDoctor_WithSpecialtyAndDate()    
    {
        // Arrange
        const string specialty = "Dermatology";
        var desiredDate = new DateOnly(2023, 8, 21);        
        var expectedId = "d6c9f315-0e35-4d5b-b25e-61a61c92d9c9";

        // Act
        var doctors = await this.doctorRepository.FindBySpecialtyWithAvailabilityAsync(specialty, desiredDate);

        // Assert
        doctors.Should().NotBeNull().And.HaveCount(1);
        doctors.Should().ContainSingle(d => d.Id.ToString() == expectedId);;
        doctors.Should().OnlyContain(d => d.OfficeHours.Any(a => a.Week == DayOfWeek.Monday));
    }
}