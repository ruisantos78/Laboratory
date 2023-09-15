using FluentAssertions;
using RuiSantos.Labs.Core.Resources;
using RuiSantos.Labs.Core.Services.Exceptions;
using RuiSantos.Labs.Tests.Asserts;

namespace RuiSantos.Labs.Tests.Stories.Administrator;

// As an administrator, I want to be able to add or remove medical specialties
public class MedicalSpecialtiesManagement
{    
    [Fact(DisplayName = "The medical specialties should be available in the system")]
    public async Task CreateMedicalSpecialtiesAsync_WithSuccess()
    {
        // Arrange
        var specialties = new List<string> {
            "Cardiology", 
            "Dermatology", 
            "Neurology", 
            "Orthopedics", 
            "Pediatrics"
        };

        var asserts = new MedicalSpecialtiesAsserts();

        // Act
        var service = asserts.GetService();
        await service.CreateMedicalSpecialtiesAsync(specialties);
        
        // Assert 
        asserts.ShouldNotLogError();

        await asserts.ShouldAddAsync(specialties);        
    }

    [Fact(DisplayName = "A log should be written when failling to add medical specialties")]
    public async Task CreateMedicalSpecialtiesAsync_WhenFailsToAdd_ThenThrowsException_AndLogsError()
    {
        // Arrange
        var specialties = new List<string>();
        
        var asserts = new MedicalSpecialtiesAsserts();
        asserts.ThrowsOnAddAsync(specialties);

        // Act
        var service = asserts.GetService();
        await service.Awaiting(x => x.CreateMedicalSpecialtiesAsync(specialties))
            .Should()
            .ThrowAsync<ServiceFailException>()
            .WithMessage(MessageResources.MedicalSpecialitiesSetFail);

        // Assert 
        asserts.ShouldLogError();
    }
    
    [Fact(DisplayName = "The specialties should be removed from the system")]
    public async Task RemoveMedicalSpecialtiesAsync_WithSuccess()
    {
        // Arrange
        const string specialty = "Orthopedics";

        var asserts = new MedicalSpecialtiesAsserts();

        // Act
        var service = asserts.GetService();
        await service.RemoveMedicalSpecialtiesAsync(specialty);

        // Assert 
        asserts.ShouldNotLogError();

        await asserts.ShouldRemoveAsync(specialty);        
    }
    
    [Fact(DisplayName = "A log should be written when fail to remove medical specialties")]
    public async Task RemoveMedicalSpecialtiesAsync_WhenFailsToRemove_ThenThrowsException_AndLogsError()
    {
        // Arrange
        var specialty = string.Empty;
        
        var asserts = new MedicalSpecialtiesAsserts();
        asserts.ThrowsOnRemoveAsync(specialty);

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
