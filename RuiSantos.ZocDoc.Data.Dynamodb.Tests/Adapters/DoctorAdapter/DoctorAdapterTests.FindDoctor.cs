namespace RuiSantos.ZocDoc.Data.Dynamodb.Tests.Adapters;

partial class DoctorAdapterTests
{
    [Fact]
    public async Task FindDoctor_WithValidLicenseNumber_ShouldReturnDoctor()
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
    public async Task FindDoctor_WithValidSpecialty_ShouldReturnDoctors()
    {
        // Arrange
        var expected = doctors.FindAll(d => d.Specialties.Contains("Cardiology"));

        // Act
        var response = await doctorAdapter.FindBySpecialityAsync("Cardiology");

        // Assert
        response.Should().NotBeNullOrEmpty();
        response.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task FindDoctor_WithInvalidLicenseNumber_ShouldReturnNull()
    {
        // Act
        var response = await doctorAdapter.FindAsync("XXX0000");

        // Assert
        response.Should().BeNull();
    }       
}

