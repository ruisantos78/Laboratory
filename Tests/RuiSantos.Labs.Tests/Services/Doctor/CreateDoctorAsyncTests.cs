using FluentAssertions;
using RuiSantos.Labs.Core.Resources;
using RuiSantos.Labs.Core.Services.Exceptions;
using RuiSantos.Labs.Tests.Asserts.Services;

namespace RuiSantos.Labs.Tests.Services.Doctor;

/// <UserStory>
/// As a doctor, I want to register and update my personal information as my contact numbers and email.
/// </UserStory>
public class CreateDoctorAsyncTests
{
    [Theory(DisplayName = "The doctor should be able to create a new record with your personal information.")]
    [InlineData("ABC123", "joe.doe@mail.com", "Joe", "Doe", new[] { "555-5555" }, new[] { "Cardiologist" })]
    public async Task CreateDoctorAsync_WithSuccess(string license, string email, string firstName, string lastName,
        string[] contactNumbers, string[] specialties)
    {
        // Arrange
        var asserts = new DoctorsServiceAsserts();

        // Act
        var service = asserts.GetService();
        await service.CreateDoctorAsync(license, email, firstName, lastName, contactNumbers, specialties);

        // Assert
        asserts.ShouldLogError(false);

        await asserts.ShouldAddAsync(x =>
            x.License == license &&
            x.Email == email &&
            x.FirstName == firstName &&
            x.LastName == lastName &&
            x.ContactNumbers.SequenceEqual(contactNumbers) &&
            x.Specialties.SequenceEqual(specialties));
    }

    [Theory(DisplayName = "A log should be writen when an unhandled exception occurs.")]
    [InlineData("ABC123", "joe.doe@mail.com", "Joe", "Doe", new[] { "555-5555" }, new[] { "Cardiologist" })]
    public async Task CreateDoctorAsync_WhenFailsToAdd_ThenThrowsException_AndLogsError(
        string license, string email, string firstName, string lastName,
        string[] contactNumbers, string[] specialties)
    {
        // Arrange
        var asserts = new DoctorsServiceAsserts();
        asserts.WhenAddAsyncThrows(license);

        // Act
        var service = asserts.GetService();
        await service.Awaiting(x =>
                x.CreateDoctorAsync(license, email, firstName, lastName, contactNumbers, specialties))
            .Should()
            .ThrowAsync<ServiceFailException>()
            .WithMessage(MessageResources.DoctorSetFail);

        // Assert
        asserts.ShouldLogError();
    }

    [Theory(DisplayName = "The doctor should not be able to create a record with invalid or missing information.")]
    [InlineData("License", "", "joe.doe@mail.com", "Joe", "Doe", new[] { "555-5555" }, new[] { "Cardiologist" })]
    [InlineData("Email", "ABC123", "<invalid email>", "Joe", "Doe", new[] { "555-5555" }, new[] { "Cardiologist" })]
    [InlineData("Email", "ABC123", "", "Joe", "Doe", new[] { "555-5555" }, new[] { "Cardiologist" })]
    [InlineData("FirstName", "ABC123", "joe.doe@mail.com", "", "Doe", new[] { "555-5555" }, new[] { "Cardiologist" })]
    [InlineData("LastName", "ABC123", "joe.doe@mail.com", "Joe", "", new[] { "555-5555" }, new[] { "Cardiologist" })]
    public async Task CreateDoctorAsync_WithInvalidInformation_ThrowsValidationFailException(string propertyName,
        string license, string email, string firstName, string lastName, string[] contactNumbers,
        string[] specialties)
    {
        // Arrange
        var asserts = new DoctorsServiceAsserts();

        // Act
        var service = asserts.GetService();
        await service.Awaiting(x =>
                x.CreateDoctorAsync(license, email, firstName, lastName, contactNumbers, specialties))
            .Should()
            .ThrowAsync<ValidationFailException>()
            .Where(ex => ex.Errors.Count(x => x.PropertyName == propertyName) == 1);

        // Assert
        asserts.ShouldLogError(false);
    }
}