using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using RuiSantos.ZocDoc.Core.Adapters;
using RuiSantos.ZocDoc.Core.Managers;
using RuiSantos.ZocDoc.Core.Managers.Exceptions;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Core.Resources;
using RuiSantos.ZocDoc.Core.Tests.Factories;

namespace RuiSantos.ZocDoc.Core.Tests;

public class MedicalSpecialtiesManagementTests
{
    private readonly Mock<IDoctorAdapter> doctorAdapterMock = new();
    private readonly Mock<IMedicalSpecialityAdapter> medicalSpecialityAdapterMock = new();
    private readonly Mock<ILogger<MedicalSpecialtiesManagement>> loggerMock = new();

    [Fact]
    public async Task CreateMedicalSpecialtiesAsync_WithValidInput_ShouldStoreMedicalSpecialties()
    {
        // Arrange
        var descriptions = SpecialtyFactory.Create().Select(s => s.Description).ToList();

        var specialties = new List<MedicalSpeciality>();

        medicalSpecialityAdapterMock.Setup(m => m.AddAsync(It.IsAny<MedicalSpeciality>()))
            .Callback<MedicalSpeciality>(specialties.Add);

        var management = new MedicalSpecialtiesManagement(medicalSpecialityAdapterMock.Object, doctorAdapterMock.Object, loggerMock.Object);

        // Act
        await management.CreateMedicalSpecialtiesAsync(descriptions);

        // Assert
        medicalSpecialityAdapterMock.Verify(m => m.AddAsync(It.IsAny<MedicalSpeciality>()), Times.Exactly(descriptions.Count));

        specialties.Should().NotBeNullOrEmpty();
        specialties.Should().HaveSameCount(descriptions);
        specialties.Select(s => s.Description).Should().BeEquivalentTo(descriptions);
    }

    [Fact]
    public async Task CreateMedicalSpecialtiesAsync_WithEmptyDescription_ShouldRaiseError()
    {
        // Arrange
        var descriptions = new List<string> { String.Empty };

        var specialties = new List<MedicalSpeciality>();

        medicalSpecialityAdapterMock.Setup(m => m.AddAsync(It.IsAny<MedicalSpeciality>()))
            .Callback<MedicalSpeciality>(specialties.Add);

        var management = new MedicalSpecialtiesManagement(medicalSpecialityAdapterMock.Object, doctorAdapterMock.Object, loggerMock.Object);

        // Act & Assert
        var failures = await management.Invoking(async m => await m.CreateMedicalSpecialtiesAsync(descriptions))
            .Should().ThrowAsync<ValidationFailException>();

        failures.Which.Errors.Should().ContainSingle()
            .Subject.PropertyName.Should().Be(nameof(MedicalSpeciality.Description));

        medicalSpecialityAdapterMock.Verify(m => m.AddAsync(It.IsAny<MedicalSpeciality>()), Times.Never);
    }

    [Fact]
    public async Task RemoveMedicalSpecialtiesAsync_WithValidInput_ShouldRemoveMedicalSpecialties()
    {
        // Arrange
        var description = "Wrong Specialty";

        var doctors = new List<Doctor>() { DoctorFactory.Create().SetSpecialties(description, "Cardiology") };

        var specialties = SpecialtyFactory.Create().ToList();
        specialties.Add(new(description));

        medicalSpecialityAdapterMock.Setup(m => m.ContainsAsync(description))
            .ReturnsAsync(specialties.Any(s => s.Description == description));

        medicalSpecialityAdapterMock.Setup(m => m.RemoveAsync(description))
            .Callback<string>(value => specialties.RemoveAll(i => i.Description == value));

        doctorAdapterMock.Setup(m => m.FindBySpecialityAsync(description))
            .ReturnsAsync(doctors.Where(d => d.Specialities.Contains(description)).ToList()); 

        doctorAdapterMock.Setup(m => m.StoreAsync(It.IsAny<Doctor>()))
            .Callback<Doctor>(value => doctors[0] = value);

        var management = new MedicalSpecialtiesManagement(medicalSpecialityAdapterMock.Object, doctorAdapterMock.Object, loggerMock.Object);

        // Act
        await management.RemoveMedicalSpecialtiesAsync(description);

        // Assert
        medicalSpecialityAdapterMock.Verify(m => m.RemoveAsync(description), Times.Once);

        specialties.Should().NotBeNullOrEmpty();
        specialties.Select(s => s.Description).Should().NotContain(description);

        doctors.Should().HaveCount(1);

        doctors[0].Should().NotBeNull();
        doctors[0].Specialities.Should().NotBeNullOrEmpty();
        doctors[0].Specialities.Should().NotContain(description);
    }

    [Fact]
    public async Task RemoveMedicalSpecialtiesAsync_WithInvalidDescription_ShouldRaiseError()
    {
        // Arrange
        var description = "Wrong Specialty";
        var specialties = SpecialtyFactory.Create().ToList();

        medicalSpecialityAdapterMock.Setup(m => m.ContainsAsync(description))
            .ReturnsAsync(specialties.Any(s => s.Description == description));

        medicalSpecialityAdapterMock.Setup(m => m.RemoveAsync(description))
            .Callback<string>(value => specialties.RemoveAll(i => i.Description == value));

        doctorAdapterMock.Setup(m => m.FindBySpecialityAsync(description))
            .ReturnsAsync(new List<Doctor>());
            
        var management = new MedicalSpecialtiesManagement(medicalSpecialityAdapterMock.Object, doctorAdapterMock.Object, loggerMock.Object);

        // Act & Assert
        await management.Invoking(async m => await m.RemoveMedicalSpecialtiesAsync(description))
            .Should().ThrowAsync<ValidationFailException>()
            .WithMessage(MessageResources.MedicalSpecialitiesDescriptionNotFound);

        medicalSpecialityAdapterMock.Verify(m => m.RemoveAsync(description), Times.Never);
    }

    [Fact]
    public async Task GetMedicalSpecialitiesAsync_WithValidInput_ReturnsExpectedResult()
    {
        // Arrange
        var specialties = SpecialtyFactory.Create().ToList();

        medicalSpecialityAdapterMock.Setup(m => m.ToListAsync()).ReturnsAsync(specialties);

        var management = new MedicalSpecialtiesManagement(medicalSpecialityAdapterMock.Object, doctorAdapterMock.Object, loggerMock.Object);

        // Act
        var result = await management.GetMedicalSpecialitiesAsync();

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().HaveSameCount(specialties);
        result.Should().BeEquivalentTo(specialties);
    }

    [Fact]
    public async Task GetMedicalSpecialitiesAsync_WithEmptyRepository_ReturnsEmptyResult()
    {
        // Arrange
        var specialties = new List<MedicalSpeciality>();

        medicalSpecialityAdapterMock.Setup(m => m.ToListAsync()).ReturnsAsync(specialties);

        var management = new MedicalSpecialtiesManagement(medicalSpecialityAdapterMock.Object, doctorAdapterMock.Object, loggerMock.Object);

        // Act
        var result = await management.GetMedicalSpecialitiesAsync();

        // Assert
        result.Should().BeEmpty();
    }
}
