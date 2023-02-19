using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using RuiSantos.ZocDoc.Core.Data;
using RuiSantos.ZocDoc.Core.Managers;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Core.Tests.Factories;

namespace RuiSantos.ZocDoc.Core.Tests;

public class DoctorManagementTests
{
    private readonly Mock<IDataContext> mockDataContext = new Mock<IDataContext>();
    private readonly Mock<IDomainContext> mockDomainContext = new Mock<IDomainContext>();
    private readonly Mock<ILogger<DoctorManagement>> mockLogger = new Mock<ILogger<DoctorManagement>>();

    [Fact]
    public async Task CreateDoctorAsync_WithValidInput_ShouldStoreDoctor()
    {
        // Arrange
        var specialty = "Neurology";
        var specialties = SpecialtyFactory.Create(specialty);
        var doctor = DoctorFactory.Create().SetSpecialties(specialty);

        var doctorStore = new List<Doctor>();
        mockDomainContext.Setup(m => m.GetMedicalSpecialtiesAsync()).ReturnsAsync(specialties);
        mockDataContext.Setup(m => m.StoreAsync(It.IsAny<Doctor>())).Callback<Doctor>(doctorStore.Add);
        
        var doctorManagement = new DoctorManagement(mockDomainContext.Object, mockDataContext.Object, mockLogger.Object);

        // Act
        await doctorManagement.CreateDoctorAsync(doctor.License, doctor.Email, doctor.FirstName, doctor.LastName, doctor.ContactNumbers, doctor.Specialties);

        // Assert
        mockDataContext.Verify(m => m.StoreAsync(It.IsAny<Doctor>()), Times.Once);

        doctorStore.Should().NotBeEmpty();
        doctorStore.Should().ContainSingle().Subject.Id.Should().NotBe(doctor.Id);
        doctorStore.Should().ContainSingle().Subject.License.Should().Be(doctor.License);
        doctorStore.Should().ContainSingle().Subject.Specialties.Should().Contain(specialty);
    }
}
