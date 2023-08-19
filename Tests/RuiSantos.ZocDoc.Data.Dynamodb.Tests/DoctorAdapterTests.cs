using RuiSantos.ZocDoc.Data.Dynamodb.Adapters;
using RuiSantos.ZocDoc.Data.Dynamodb.Tests.Fixtures;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Tests;

public class DoctorAdapterTests: IClassFixture<DatabaseFixture>
{
    private readonly DoctorAdapter doctorAdapter;
    
    public DoctorAdapterTests(DatabaseFixture database)
    {
        this.doctorAdapter = new(database.Client);
    }

    [Theory(DisplayName = "Should find doctor by license. ")]
    [InlineData("d6c9f315-0e35-4d5b-b25e-61a61c92d9c9", "XYZ002")] 
    [InlineData("8a6151c7-9122-4f1b-a1e7-85e981c17a14", "DEF003")] 
    public async Task ShouldFindDoctorByLicenseNumber(string doctorId, string licenseNumber)
    {
        // Arrange

        // Act
        var doctor = await this.doctorAdapter.FindAsync(licenseNumber);

        // Assert
        doctor.Should().NotBeNull();
        doctor!.License.Should().Be(licenseNumber);
        doctor!.Id.Should().Be(doctorId);
    }
}