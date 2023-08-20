using Microsoft.Extensions.Logging;
using RuiSantos.ZocDoc.Core.Services.Exceptions;
using RuiSantos.ZocDoc.Core.Resources;
using RuiSantos.ZocDoc.Core.Tests.Repositories;
using RuiSantos.ZocDoc.Core.Tests.Builders;

namespace RuiSantos.ZocDoc.Core.Tests;

public class MedicalSpecialtiesServiceTests
{
    private readonly DoctorRepositoryMock doctorAdapterMock = new();
    private readonly MedicalSpecialityRepositoryMock medicalSpecialityAdapterMock = new();
    private readonly Mock<ILogger<MedicalSpecialtiesService>> loggerMock = new();

    private IMedicalSpecialtiesService Management => new MedicalSpecialtiesService(medicalSpecialityAdapterMock.Object, doctorAdapterMock.Object, loggerMock.Object);

    [Fact]
    public async Task CreateMedicalSpecialtiesAsync_WithValidInput_ShouldStoreMedicalSpecialties()
    {
        // Arrange
        var specialties = new List<MedicalSpecialty>();
        medicalSpecialityAdapterMock.SetAddAsyncCallback(specialties.Add);

        // Act
        var descriptions = new List<string>
        {
            "Cardiology",
            "Dermatology",
            "Endocrinology",
            "Gastroenterology",
            "Geriatrics"
        };

        await Management.CreateMedicalSpecialtiesAsync(descriptions);

        // Assert
        medicalSpecialityAdapterMock.ShouldAddAsync(descriptions.Count);

        specialties.Should().NotBeNullOrEmpty().And.HaveSameCount(descriptions);
        specialties.Should().AllSatisfy(me => me.Description.Should().BeOneOf(descriptions));
    }

    [Fact]
    public async Task CreateMedicalSpecialtiesAsync_WithEmptyDescription_ShouldRaiseError()
    {
        // Arrange
        var specialties = new List<MedicalSpecialty>();
        medicalSpecialityAdapterMock.SetAddAsyncCallback(specialties.Add);

        // Act & Assert
        var descriptions = new List<string> { String.Empty };

        var failures = await Management.Awaiting(m => m.CreateMedicalSpecialtiesAsync(descriptions))
            .Should().ThrowAsync<ValidationFailException>();

        failures.Which.Errors.Should()
            .ContainSingle(e => e.PropertyName == nameof(MedicalSpecialty.Description));

        medicalSpecialityAdapterMock.ShouldNotAddAsync();
    }

    [Fact]
    public async Task RemoveMedicalSpecialtiesAsync_WithValidInput_ShouldRemoveMedicalSpecialties()
    {
        // Arrange
        var doctors = DoctorBuilder.Dummy()
            .AddSpecialties("Cardiology", "Wrong Specialty")
            .BuildList();

        doctorAdapterMock.SetStoreAsyncCallback(value => {
            var index = doctors.FindIndex(d => d.Id == value.Id);
            doctors[index] = value;
        });

        doctorAdapterMock.SetFindBySpecialtyAsyncReturns(description =>
            doctors.FindAll(d => d.Specialties.Contains(description)));

        var specialties = SpecialtiesBuilder.Dummy()
            .AddSpecialties("Wrong Specialty")
            .Build();

        medicalSpecialityAdapterMock.SetContainsAsyncReturns(description =>
            specialties.Any(s => s.Description == description));

        medicalSpecialityAdapterMock.SetRemoveAsyncCallback(description =>
            specialties.RemoveAll(i => i.Description == description));

        // Act
        var description = "Wrong Specialty";
        await Management.RemoveMedicalSpecialtiesAsync(description);

        // Assert
        medicalSpecialityAdapterMock.ShouldRemoveAsync(description);
        doctorAdapterMock.ShouldStoreAsync();

        specialties.Should().NotBeNullOrEmpty();
        specialties.Should().OnlyContain(s => s.Description != description);

        doctors.Should().NotBeNullOrEmpty().And.HaveCount(1);
        doctors.Should().OnlyContain(d => !d.Specialties.Contains(description));
    }

    [Fact]
    public async Task RemoveMedicalSpecialtiesAsync_WithInvalidDescription_ShouldRaiseError()
    {
        // Arrange        
        var specialties = SpecialtiesBuilder.Dummy().Build();

        medicalSpecialityAdapterMock.SetContainsAsyncReturns(item =>
            specialties.Any(s => s.Description == item));

        medicalSpecialityAdapterMock.SetRemoveAsyncCallback(item =>
            specialties.RemoveAll(s => s.Description == item));

        doctorAdapterMock.SetFindBySpecialtyAsyncReturns(new List<Doctor>());

        // Act & Assert
        await Management.Awaiting(m => m.RemoveMedicalSpecialtiesAsync("Wrong Specialty"))
            .Should().ThrowAsync<ValidationFailException>()
            .WithMessage(MessageResources.MedicalSpecialitiesDescriptionNotFound);

        medicalSpecialityAdapterMock.ShouldNotRemoveAsync();
    }

    [Fact]
    public async Task GetMedicalSpecialitiesAsync_WithValidInput_ReturnsExpectedResult()
    {
        // Arrange
        var specialties = SpecialtiesBuilder.Dummy().Build();

        medicalSpecialityAdapterMock.SetToListAsyncReturns(specialties);

        // Act
        var result = await Management.GetMedicalSpecialitiesAsync();

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().HaveSameCount(specialties);
        result.Should().BeEquivalentTo(specialties);
    }

    [Fact]
    public async Task GetMedicalSpecialitiesAsync_WithEmptyRepository_ReturnsEmptyResult()
    {
        // Arrange
        medicalSpecialityAdapterMock.SetToListAsyncReturns(new List<MedicalSpecialty>());

        // Act
        var result = await Management.GetMedicalSpecialitiesAsync();

        // Assert
        result.Should().BeEmpty();
    }
}
