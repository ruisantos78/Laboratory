using FluentAssertions;
using NSubstitute;
using RuiSantos.Labs.Core;
using RuiSantos.Labs.Core.Services.Exceptions;
using RuiSantos.Labs.Tests.Factories;

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

        var factory = new MedicalSpecialtiesFactory();

        // Act
        var service = factory.CreateService();
        await service.CreateMedicalSpecialtiesAsync(specialties);
        
        // Assert 
        await factory.Repository.Received().AddAsync(specialties);
        factory.Cache.Received().ClearMedicalSpecialties();
        factory.Logger.DidNotReceiveWithAnyArgs().Fail(default);
    }
    
    [Fact(DisplayName = "Log when fail to add medical specialties")]
    public async Task AddMedicalSpecialties_ShouldWriteLog_WhenThrowsExceptions()
    {
        // Arrange
        var specialties = new List<string>();
        
        var factory = new MedicalSpecialtiesFactory();

        factory.Repository
            .When(x => x.AddAsync(specialties))
            .Throw<Exception>();
        
        // Act
        var service = factory.CreateService();
        await service.Awaiting(x => x.CreateMedicalSpecialtiesAsync(specialties))
            .Should()
            .ThrowAsync<ServiceFailException>()
            .WithMessage("Failed to store a medical speciality.");

        // Assert 
        await factory.Repository.Received().AddAsync(specialties);
        factory.Cache.DidNotReceive().ClearMedicalSpecialties();
        factory.Logger.ReceivedWithAnyArgs().Fail(default);
    }
    
    [Fact(DisplayName = "Remove a medical specialty")]
    public async Task RemoveMedicalSpecialty_WithSuccess()
    {
        // Arrange
        var specialty = "Orthopedics";

        var factory = new MedicalSpecialtiesFactory();

        // Act
        var service = factory.CreateService();
        await service.RemoveMedicalSpecialtiesAsync(specialty);

        // Assert 
        await factory.Repository.Received().RemoveAsync(specialty);
        factory.Cache.Received().ClearMedicalSpecialties();
        factory.Logger.DidNotReceiveWithAnyArgs().Fail(default);
    }
    
    [Fact(DisplayName = "Log when fail to remove a medical specialty")]
    public async Task RemoveMedicalSpecialty_ShouldWriteLog_WhenThrowsExceptions()
    {
        // Arrange
        var specialty = string.Empty;
        
        var factory = new MedicalSpecialtiesFactory();
        
        factory.Repository
            .When(x => x.RemoveAsync(specialty))
            .Throw<Exception>();
        
        // Act
        var service = factory.CreateService();
        await service.Awaiting(x => x.RemoveMedicalSpecialtiesAsync(specialty))
            .Should()
            .ThrowAsync<ServiceFailException>()
            .WithMessage("Failed to store a medical speciality.");
        
        // Assert 
        await factory.Repository.Received().RemoveAsync(specialty);
        factory.Cache.DidNotReceive().ClearMedicalSpecialties();
        factory.Logger.ReceivedWithAnyArgs().Fail(default);
    }
}
