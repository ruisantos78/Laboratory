using FluentAssertions;
using NSubstitute;
using RuiSantos.Labs.Core.Resources;
using RuiSantos.Labs.Core.Services.Exceptions;
using RuiSantos.Labs.Tests.Asserts;
using TDoctor = RuiSantos.Labs.Core.Models.Doctor;

namespace RuiSantos.Labs.Tests.Stories.Doctor;

// As a doctor, I want to register and update my personal information as my contact numbers and email.
public class DoctorInformationManagement
{
    private static readonly string[] Specialties = { "Cardiologist" };

    public static IEnumerable<object[]> GetDoctors() => new List<object[]>()
    {
        new object[] { "ABC123", "joe.doe@mail.com", "Joe", "Doe", new[] { "555-5555" }, new[] { "Cardiologist" } }
    };

    [Theory(DisplayName = "The doctor should be able to create a new record with your personal information.")]
    [MemberData(nameof(GetDoctors))]
    public async Task CreateDoctorAsync_WithSuccess(
        string license, string email, string firstName, string lastName,
        string[] contactNumbers, string[] specialties)
    {
        // Arrange
        var asserts = new DoctorsAsserts(Specialties);
        
        // Act
        var service = asserts.GetService();
        await service.CreateDoctorAsync(license, email, firstName, lastName, contactNumbers, specialties);

        // Assert
        asserts.ShouldNotLogError();

        await asserts.DoctorRepository.Received()
            .StoreAsync(Arg.Any<TDoctor>());        
    }

    [Theory(DisplayName = "A log shoud be writen when an unhandled exception occurs.")]
    [MemberData(nameof(GetDoctors))]
    public async Task CreateDoctorAsync_WithUnhandledException_ShouldLogsError(string license, string email, string firstName, string lastName,
    string[] contactNumbers, string[] specialties)
    {
        // Arrange
        var asserts = new DoctorsAsserts(Specialties);

        asserts.DoctorRepository
            .When(x => x.StoreAsync(Arg.Any<Core.Models.Doctor>()))
            .Throw<Exception>();

        // Act
        var service = asserts.GetService();
        await service.Awaiting(x => x.CreateDoctorAsync(license, email, firstName, lastName, contactNumbers, specialties))
            .Should()
            .ThrowAsync<ServiceFailException>()
            .WithMessage(MessageResources.DoctorSetFail);

        // Assert
        asserts.ShouldLogError();
    }

    [Theory(DisplayName = "The doctor should not be able to create a record with invalid or missing information.")]
    [InlineData("License", null, "joe.doe@mail.com", "Joe", "Doe", new[] { "555-5555" }, new[] { "Cardiologist" })]
    [InlineData("Email", "ABC123", "<invalid email>", "Joe", "Doe", new[] { "555-5555" }, new[] { "Cardiologist" })]
    [InlineData("Email", "ABC123", null, "Joe", "Doe", new[] { "555-5555" }, new[] { "Cardiologist" })]
    [InlineData("FirstName", "ABC123", "joe.doe@mail.com", null, "Doe", new[] { "555-5555" }, new[] { "Cardiologist" })]
    [InlineData("LastName", "ABC123", "joe.doe@mail.com", "Joe", null, new[] { "555-5555" }, new[] { "Cardiologist" })]
    [InlineData("Specialties[0]", "ABC123", "joe.doe@mail.com", "Joe", "Doe", new[] { "555-5555" }, new[] { "<invalid specialty>" })]
    public async Task CreateDoctorAsync_WithInvalidInformation_ThrowsValidationFailException(
        string propertyName,
        string license, string email, string firstName, string lastName,
        string[] contactNumbers, string[] specialties)
    {
        // Arrange
        var asserts = new DoctorsAsserts(Specialties);

        // Act
        var service = asserts.GetService();
        await service.Awaiting(x => x.CreateDoctorAsync(license, email, firstName, lastName, contactNumbers, specialties))
            .Should()
            .ThrowAsync<ValidationFailException>()
            .Where(ex => ex.Errors.Single().PropertyName == propertyName);

        // Assert
        asserts.ShouldNotLogError();
    }
}