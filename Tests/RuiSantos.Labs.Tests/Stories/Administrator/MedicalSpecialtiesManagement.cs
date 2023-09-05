using FluentAssertions;
using NSubstitute;
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
        asserts.ShouldClearCache();
        asserts.ShouldNotLogError();
        
        await asserts.Repository.Received().AddAsync(specialties);        
    }

    [Fact(DisplayName = "A log should be written when failling to add medical specialties")]
    public async Task CreateMedicalSpecialtiesAsync_WhenThrowsExceptions_ShouldWriteLog()
    {
        // Arrange
        var specialties = new List<string>();
        
        var asserts = new MedicalSpecialtiesAsserts();

        asserts.Repository
            .When(x => x.AddAsync(specialties))
            .Throw<Exception>();
        
        // Act
        var service = asserts.GetService();
        await service.Awaiting(x => x.CreateMedicalSpecialtiesAsync(specialties))
            .Should()
            .ThrowAsync<ServiceFailException>()
            .WithMessage(MessageResources.MedicalSpecialitiesSetFail);

        // Assert 
        asserts.ShouldNotClearCache();
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
        asserts.ShouldClearCache();
        asserts.ShouldNotLogError();

        await asserts.Repository.Received().RemoveAsync(specialty);        
    }
    
    [Fact(DisplayName = "A log should be written when fail to remove medical specialties")]
    public async Task RemoveMedicalSpecialtiesAsync_WhenThrowsExceptions_ShouldWriteLog()
    {
        // Arrange
        var specialty = string.Empty;
        
        var asserts = new MedicalSpecialtiesAsserts();

        asserts.Repository
            .When(x => x.RemoveAsync(specialty))
            .Throw<Exception>();

        // Act
        var service = asserts.GetService();
        await service.Awaiting(x => x.RemoveMedicalSpecialtiesAsync(specialty))
            .Should()
            .ThrowAsync<ServiceFailException>()
            .WithMessage(MessageResources.MedicalSpecialitiesSetFail);
        
        // Assert 
        asserts.ShouldNotClearCache();
        asserts.ShouldLogError();
    }
}
