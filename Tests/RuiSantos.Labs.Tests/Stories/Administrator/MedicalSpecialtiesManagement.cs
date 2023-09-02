using FluentAssertions;
using NSubstitute;
using RuiSantos.Labs.Core;
using RuiSantos.Labs.Core.Services.Exceptions;
using RuiSantos.Labs.Tests.Asserts;

namespace RuiSantos.Labs.Tests.Stories.Administrator;

// As an administrator, I want to be able to add or remove medical specialties
public class MedicalSpecialtiesManagement
{    
    [Fact(DisplayName = "Add medical specialties")]
    public async Task AddMedicalSpecialties_WithSuccess()
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
        await asserts.Repository.Received().AddAsync(specialties);
        asserts.Cache.Received().ClearMedicalSpecialties();        
        asserts.Logger.DidNotReceiveWithAnyArgs().Fail(default);
    }

    [Fact(DisplayName = "Log when fail to add medical specialties")]
    public async Task AddMedicalSpecialties_ShouldWriteLog_WhenThrowsExceptions()
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
            .WithMessage("Failed to store a medical speciality.");

        // Assert 
        await asserts.Repository.Received().AddAsync(specialties);
        asserts.Cache.DidNotReceive().ClearMedicalSpecialties();
        asserts.Logger.ReceivedWithAnyArgs().Fail(default);
    }
    
    [Fact(DisplayName = "Remove a medical specialty")]
    public async Task RemoveMedicalSpecialty_WithSuccess()
    {
        // Arrange
        var specialty = "Orthopedics";

        var asserts = new MedicalSpecialtiesAsserts();

        // Act
        var service = asserts.GetService();
        await service.RemoveMedicalSpecialtiesAsync(specialty);

        // Assert 
        await asserts.Repository.Received().RemoveAsync(specialty);
        asserts.Cache.Received().ClearMedicalSpecialties();
        asserts.Logger.DidNotReceiveWithAnyArgs().Fail(default);
    }
    
    [Fact(DisplayName = "Log when fail to remove a medical specialty")]
    public async Task RemoveMedicalSpecialty_ShouldWriteLog_WhenThrowsExceptions()
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
            .WithMessage("Failed to store a medical speciality.");
        
        // Assert 
        await asserts.Repository.Received().RemoveAsync(specialty);
        asserts.Cache.DidNotReceive().ClearMedicalSpecialties();
        asserts.Logger.ReceivedWithAnyArgs().Fail(default);
    }
}
