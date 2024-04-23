using FluentAssertions;
using RuiSantos.Labs.Core.Resources;
using RuiSantos.Labs.Core.Services.Exceptions;
using RuiSantos.Labs.Tests.Asserts.Services;

namespace RuiSantos.Labs.Tests.Services.MedicalSpecialites;

// As an administrator, I want to be able to add or remove medical specialties
public class CreateMedicalSpecialtiesAsyncTests
{
    private static readonly List<string> MedicalSpecialties = new() {
        "Cardiology",
        "Dermatology",
        "Neurology",
        "Orthopedics",
        "Pediatrics"
    };
    
    [Fact(DisplayName = "The medical specialties should be available in the system")]
    public async Task CreateMedicalSpecialtiesAsync_WithSuccess()
    {
        // Arrange
        var asserts = new MedicalSpecialtiesServiceAsserts();

        // Act
        var service = asserts.GetService();
        await service.CreateMedicalSpecialtiesAsync(MedicalSpecialties);

        // Assert 
        asserts.ShouldLogError(false);

        await asserts.ShouldAddAsync(MedicalSpecialties);
    }

    [Fact(DisplayName = "A log should be written when failling to add medical specialties")]
    public async Task CreateMedicalSpecialtiesAsync_WhenFailsToAdd_ThenThrowsException_AndLogsError()
    {
        // Arrange
        var asserts = new MedicalSpecialtiesServiceAsserts();
        asserts.WhenAddAsyncThrows(MedicalSpecialties);

        // Act
        var service = asserts.GetService();
        await service.Awaiting(x => x.CreateMedicalSpecialtiesAsync(MedicalSpecialties))
            .Should()
            .ThrowAsync<ServiceFailException>()
            .WithMessage(MessageResources.MedicalSpecialitiesSetFail);

        // Assert 
        asserts.ShouldLogError();
    }
}
