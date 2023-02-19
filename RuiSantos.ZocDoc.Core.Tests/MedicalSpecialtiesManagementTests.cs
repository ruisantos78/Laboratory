using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using RuiSantos.ZocDoc.Core.Data;
using RuiSantos.ZocDoc.Core.Managers;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Core.Tests.Factories;
using System.Linq.Expressions;

namespace RuiSantos.ZocDoc.Core.Tests;

public class MedicalSpecialtiesManagementTests
{
    private readonly Mock<IDataContext> mockDataContext = new Mock<IDataContext>();
    private readonly Mock<ILogger<MedicalSpecialtiesManagement>> mockLogger = new Mock<ILogger<MedicalSpecialtiesManagement>>();

    [Fact]
    public async Task CreateMedicalSpecialtiesAsync_WithValidInput_ShouldStoreMedicalSpecialties()
    {
        // Arrange
        var descriptions = SpecialtyFactory.Create().Select(s => s.Description);

        var specialties = new List<MedicalSpeciality>();

        mockDataContext.Setup(m => m.StoreAsync(It.IsAny<MedicalSpeciality>()))
            .Callback<MedicalSpeciality>(specialties.Add);

        var management = new MedicalSpecialtiesManagement(mockDataContext.Object, mockLogger.Object);

        // Act
        await management.CreateMedicalSpecialtiesAsync(descriptions);

        // Assert
        mockDataContext.Verify(m => m.StoreAsync(It.IsAny<MedicalSpeciality>()), Times.Exactly(descriptions.Count()));

        specialties.Should().NotBeNullOrEmpty();
        specialties.Should().HaveSameCount(descriptions);
        specialties.Select(s => s.Description).Should().BeEquivalentTo(descriptions);
    }

    [Fact]
    public async Task RemoveMedicalSpecialtiesAsync_WithValidInput_ShouldRemoveMedicalSpecialties()
    {
        // Arrange
        var description = "Wrong Specialty";

        var specialties = SpecialtyFactory.Create().ToList();
        specialties.Add(new(description));

        mockDataContext.Setup(m => m.QueryAsync(It.IsAny<Expression<Func<MedicalSpeciality, bool>>>()))
            .ReturnsAsync((Expression<Func<MedicalSpeciality, bool>> expression) => specialties.Where(expression.Compile()).ToList());

        mockDataContext.Setup(m => m.RemoveAsync<MedicalSpeciality>(It.IsAny<Guid>()))
            .Callback<Guid>(value => specialties.RemoveAll(i => i.Id == value));

        var management = new MedicalSpecialtiesManagement(mockDataContext.Object, mockLogger.Object);

        // Act
        await management.RemoveMedicalSpecialtiesAsync(description);

        // Assert
        mockDataContext.Verify(m => m.RemoveAsync<MedicalSpeciality>(It.IsAny<Guid>()), Times.Once);

        specialties.Should().NotBeNullOrEmpty();
        specialties.Select(s => s.Description).Should().NotContain(description);
    }

    [Fact]
    public async Task GetMedicalSpecialitiesAsync_WithValidInput_ReturnsExpectedResult()
    {
        // Arrange
        var specialties = SpecialtyFactory.Create().ToList();

        mockDataContext.Setup(m => m.ListAsync<MedicalSpeciality>()).ReturnsAsync(specialties);

        var management = new MedicalSpecialtiesManagement(mockDataContext.Object, mockLogger.Object);

        // Act
        var result = await management.GetMedicalSpecialitiesAsync();

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().HaveSameCount(specialties);
        result.Should().BeEquivalentTo(specialties);
    }
}
