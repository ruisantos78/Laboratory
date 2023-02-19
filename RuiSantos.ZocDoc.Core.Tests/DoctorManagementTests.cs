using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using RuiSantos.ZocDoc.Core.Data;
using RuiSantos.ZocDoc.Core.Managers;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Core.Tests.Factories;
using System.ComponentModel;
using System;
using System.Linq.Expressions;

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
        var args = DoctorFactory.Create().SetSpecialties(specialty);
        var doctor = DoctorFactory.Empty();        

        mockDomainContext.Setup(m => m.GetMedicalSpecialtiesAsync()).ReturnsAsync(SpecialtyFactory.Create(specialty));
        mockDataContext.Setup(m => m.StoreAsync(It.IsAny<Doctor>())).Callback<Doctor>(value => doctor = value);
        
        var doctorManagement = new DoctorManagement(mockDomainContext.Object, mockDataContext.Object, mockLogger.Object);

        // Act
        await doctorManagement.CreateDoctorAsync(args.License, args.Email, args.FirstName, args.LastName, args.ContactNumbers, args.Specialties);

        // Assert
        mockDataContext.Verify(m => m.StoreAsync(It.IsAny<Doctor>()), Times.Once);
        
        doctor.Should().NotBeNull();
        doctor.Id.Should().NotBe(args.Id).And.NotBeEmpty();
        doctor.License.Should().Be(args.License);
        doctor.Email.Should().Be(args.Email);
        doctor.FirstName.Should().Be(args.FirstName);
        doctor.LastName.Should().Be(args.LastName);
        doctor.ContactNumbers.Should().BeEquivalentTo(args.ContactNumbers);
        doctor.Specialties.Should().BeEquivalentTo(args.Specialties);
    }

    [Fact]
    public async Task SetOfficeHoursAsync_WithValidInput_ShouldUpdateDoctor()
    {
        // Arrange
        var license = "ABC123";
        var dayOfWeek = DayOfWeek.Monday;
        var hours = new[] {
            new TimeSpan(8,0,0),
            new TimeSpan(8,3,0),
            new TimeSpan(9,0,0),
            new TimeSpan(9,3,0)
        };

        var doctor = DoctorFactory.Empty();
        mockDataContext.Setup(m => m.FindAsync(It.IsAny<Expression<Func<Doctor, bool>>>())).ReturnsAsync(DoctorFactory.Create(license));
        mockDataContext.Setup(m => m.StoreAsync(It.IsAny<Doctor>())).Callback<Doctor>(value => doctor = value);

        var doctorManagement = new DoctorManagement(mockDomainContext.Object, mockDataContext.Object, mockLogger.Object);
        
        // Act
        await doctorManagement.SetOfficeHoursAsync(license, dayOfWeek, hours);

        // Assert
        mockDataContext.Verify(m => m.StoreAsync(It.IsAny<Doctor>()), Times.Once);

        doctor.Should().NotBeNull();
        doctor.Id.Should().NotBeEmpty();
        doctor.License.Should().Be(license);
        doctor.OfficeHours.Should().NotBeNullOrEmpty();
        doctor.OfficeHours.Should().ContainSingle().Subject.Week.Should().Be(dayOfWeek);
        doctor.OfficeHours.Should().ContainSingle().Subject.Hours.Should().BeSameAs(hours);
    }

    [Fact]
    public async Task GetDoctorByLicenseAsync_WithValidInput_ReturnsExpectedResult()
    {
        // Arrange
        var license = "ABC123";

        mockDataContext.Setup(m => m.FindAsync(It.IsAny<Expression<Func<Doctor, bool>>>())).ReturnsAsync(DoctorFactory.Create(license));

        var doctorManagement = new DoctorManagement(mockDomainContext.Object, mockDataContext.Object, mockLogger.Object);

        // Act
        var doctor = await doctorManagement.GetDoctorByLicenseAsync(license);

        // Assert
        doctor?.Should().NotBeNull();
        doctor?.Id.Should().NotBeEmpty();
        doctor?.License.Should().Be(license);
    }
}
