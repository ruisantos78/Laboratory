using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Data.Dynamodb.Adapters;
using RuiSantos.ZocDoc.Data.Dynamodb.Tests.Fixtures;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Tests;

public class DoctorAdapterTests: IClassFixture<DatabaseFixture>
{
    private readonly DoctorAdapter doctorAdapter;
    private readonly List<Doctor> doctors;

    public DoctorAdapterTests(DatabaseFixture database)
    {
        this.doctorAdapter = new(database.Client);
        this.doctors = database.Doctors;
    }

    [Fact]
    public async Task ShouldFindDoctor_WithLicenseNumber()
    {
        // Arrange
        var doctor = doctors.Find(d => d.License == "ABC001");

        // Act
        var response = await doctorAdapter.FindAsync("ABC001");

        // Assert
        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(doctor);
    }

    [Fact]
    public async Task ShouldFindDoctor_WithSpecialty()
    {
        // Arrange
        var expected = doctors.FindAll(d => d.Specialties.Contains("Cardiology"));

        // Act
        var response = await doctorAdapter.FindBySpecialityAsync("Cardiology");

        // Assert
        response.Should().NotBeNullOrEmpty();
        response.Should().BeEquivalentTo(expected);
    }
}

