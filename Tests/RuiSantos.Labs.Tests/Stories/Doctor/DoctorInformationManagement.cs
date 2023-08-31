using FluentAssertions;
using NSubstitute;
using RuiSantos.Labs.Core;
using RuiSantos.Labs.Core.Services.Exceptions;
using RuiSantos.Labs.Tests.Factories;

namespace RuiSantos.Labs.Tests.Stories.Doctor;

// As a doctor, I want to register and update my personal information as my contact numbers and email.
public class DoctorInformationManagement
{
    private readonly IReadOnlySet<string>? cachedSpecialties = new HashSet<string>()
    {
        "Cardiologist"
    };
    
    [Theory(DisplayName = "Register the doctor personal informations")]
    [InlineData("ABC123", "joe.doe@mail.com", "Joe", "Doe", new[] { "555-5555" }, new[] { "Cardiologist" })]
    public async Task RegisterDoctorPersonalInformations_WithSuccess(
        string license, string email, string firstName, string lastName,
        string[] contactNumbers, string[] specialties)
    {
        // Arrange
        var factory = new DoctorsFactory();
        factory.Cache.GetMedicalSpecialtiesAsync().Returns(Task.FromResult(cachedSpecialties));
        
        // Act
        var service = factory.CreateService();
        await service.CreateDoctorAsync(license, email, firstName, lastName, contactNumbers, specialties);
        
        // Assert
        await factory.DoctorRepository.Received().StoreAsync(Arg.Any<Core.Models.Doctor>());
        factory.Logger.DidNotReceiveWithAnyArgs().Fail(default);
    }
    
    [Theory(DisplayName = "Register the doctor personal informations")]
    [InlineData("License", null, "joe.doe@mail.com", "Joe", "Doe", new[] { "555-5555" }, new[] { "Cardiologist" })]
    [InlineData("Email", "ABC123", "<invalid email>", "Joe", "Doe", new[] { "555-5555" }, new[] { "Cardiologist" })]
    [InlineData("Email", "ABC123", null, "Joe", "Doe", new[] { "555-5555" }, new[] { "Cardiologist" })]
    [InlineData("FirstName", "ABC123", "joe.doe@mail.com", null, "Doe", new[] { "555-5555" }, new[] { "Cardiologist" })]
    [InlineData("LastName", "ABC123", "joe.doe@mail.com", "Joe", null, new[] { "555-5555" }, new[] { "Cardiologist" })]
    [InlineData("Specialties[0]", "ABC123", "joe.doe@mail.com", "Joe", "Doe", new[] { "555-5555" }, new[] { "<invalid specialty>" })]
    public async Task RegisterDoctorPersonalInformations_ShouldFails_OnValidations(
        string propertyName,
        string license, string email, string firstName, string lastName,
        string[] contactNumbers, string[] specialties)
    {
        // Arrange
        var factory = new DoctorsFactory();
        factory.Cache.GetMedicalSpecialtiesAsync().Returns(Task.FromResult(cachedSpecialties));
        
        // Act
        var service = factory.CreateService();
        await service.Awaiting(x =>
                x.CreateDoctorAsync(license, email, firstName, lastName, contactNumbers, specialties))
            .Should()
            .ThrowAsync<ValidationFailException>()
            .Where(ex => ex.Errors.Single().PropertyName == propertyName);
        
        // Assert
        factory.Logger.DidNotReceiveWithAnyArgs().Fail(default);
    }
}