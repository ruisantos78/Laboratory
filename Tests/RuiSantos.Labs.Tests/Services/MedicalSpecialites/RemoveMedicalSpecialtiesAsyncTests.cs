using FluentAssertions;
using RuiSantos.Labs.Core.Resources;
using RuiSantos.Labs.Core.Services.Exceptions;
using RuiSantos.Labs.Tests.Asserts.Services;

namespace RuiSantos.Labs.Tests.Services.MedicalSpecialites;

// As an administrator, I want to be able to add or remove medical specialties
public class RemoveMedicalSpecialtiesAsyncTests
{    
    [Fact(DisplayName = "The specialties should be removed from the system")]
    public async Task RemoveMedicalSpecialtiesAsync_WithSuccess()
    {
        // Arrange
        const string specialty = "Orthopedics";

        var asserts = new MedicalSpecialtiesServiceAsserts();

        // Act
        var service = asserts.GetService();
        await service.RemoveMedicalSpecialtiesAsync(specialty);

        // Assert 
        asserts.ShouldLogError(false);

        await asserts.ShouldRemoveAsync(specialty);        
    }
    
    [Fact(DisplayName = "A log should be written when fail to remove medical specialties")]
    public async Task RemoveMedicalSpecialtiesAsync_WhenFailsToRemove_ThenThrowsException_AndLogsError()
    {
        // Arrange
        var specialty = string.Empty;
        
        var asserts = new MedicalSpecialtiesServiceAsserts();
        asserts.WhenRemoveAsyncThrows(specialty);

        // Act
        var service = asserts.GetService();
        await service.Awaiting(x => x.RemoveMedicalSpecialtiesAsync(specialty))
            .Should()
            .ThrowAsync<ServiceFailException>()
            .WithMessage(MessageResources.MedicalSpecialitiesSetFail);
        
        // Assert 
        asserts.ShouldLogError();
    }
}
