using FluentAssertions;
using RuiSantos.Labs.Core.Resources;
using RuiSantos.Labs.Core.Services.Exceptions;
using RuiSantos.Labs.Tests.Asserts.Builders;
using RuiSantos.Labs.Tests.Asserts.Services;

namespace RuiSantos.Labs.Tests.Services.Doctor;

/// <UserStory>
/// As a doctor, I want to be able to define and manage my work hours.
/// </UserStory>
public class SetOfficeHoursAsyncTests
{
    private static readonly string[] MorningHours = { "09:00", "09:30", "10:00", "10:30", "11:00", "11:30" };
    private static readonly string[] AfternoonHours = { "13:00", "13:30", "14:00", "14:30", "15:00", "15:30" };

    public static IEnumerable<object[]> OfficeHoursMemberData => new List<object[]> {
        new object[] { DayOfWeek.Monday, MorningHours },
        new object[] { DayOfWeek.Wednesday, MorningHours },
        new object[] { DayOfWeek.Friday, MorningHours },
    };

    [Theory(DisplayName = "The doctor should be able the set with hours they attend by week days")]
    [MemberData(nameof(OfficeHoursMemberData))]
    public async Task SetOfficeHoursAsync_WithSuccess(DayOfWeek dayOfWeek, string[] hours)
    {
        // Arrange
        var doctor = new DoctorBuilder().Build();
        var officeHours = hours.Select(TimeSpan.Parse).ToArray();

        var asserts = new DoctorsServiceAsserts();
        asserts.OnFindAsyncReturns(doctor.Id, result: doctor);
        
        // Act
        var service = asserts.GetService();
        await service.SetOfficeHoursAsync(doctor.Id, dayOfWeek, officeHours);

        // Assert
        asserts.ShouldNotLogError();

        await asserts.ShouldStoreAsync(x =>
            x.Id == doctor.Id &&
            x.OfficeHours.All(y => y.Week == dayOfWeek && y.Hours.SequenceEqual(officeHours)));
    }

    [Theory(DisplayName = "The doctor should be able to modify their weekly schedule by changing the office hours.")]
    [MemberData(nameof(OfficeHoursMemberData))]
    public async Task SetOfficeHoursAsync_WhenHasOfficeHours_ShouldReplace_WithSuccess(DayOfWeek dayOfWeek, string[] hours)
    {
        // Arrange
        var officeHours = hours.Select(TimeSpan.Parse).ToArray();
        var afternoon = AfternoonHours.Select(TimeSpan.Parse).ToArray();
        
        var doctor = new DoctorBuilder()
            .WithOfficeHours(DayOfWeek.Monday, AfternoonHours)
            .WithOfficeHours(DayOfWeek.Wednesday, AfternoonHours)
            .WithOfficeHours(DayOfWeek.Friday, AfternoonHours)
            .Build();
        
        var asserts = new DoctorsServiceAsserts();
        asserts.OnFindAsyncReturns(doctor.Id, doctor);

        // Act
        var service = asserts.GetService();
        await service.SetOfficeHoursAsync(doctor.Id, dayOfWeek, officeHours);

        // Assert
        asserts.ShouldNotLogError();

        await asserts.ShouldStoreAsync(x => 
            x.Id == doctor.Id && 
            x.OfficeHours.All(y => 
                (y.Week == dayOfWeek && y.Hours.SequenceEqual(officeHours)) ||
                (y.Week != dayOfWeek && y.Hours.SequenceEqual(afternoon))
            ));
    }

    [Theory(DisplayName = "It should not be possible to establish office hours for a doctor who is not registered.")]    
    [MemberData(nameof(OfficeHoursMemberData))]
    public async Task SetOfficeHoursAsync_WhenDoctorDoesNotExist_ThrownValidationFailException(DayOfWeek dayOfWeek, string[] hours)
    {
        // Arrange
        var doctorId = Guid.NewGuid();
        var officeHours = hours.Select(TimeSpan.Parse).ToArray();
        
        var asserts = new DoctorsServiceAsserts();
        asserts.OnFindAsyncReturns(doctorId, default);
        
        // Act & Assert
        var service = asserts.GetService();
        await service.Awaiting(x => x.SetOfficeHoursAsync(doctorId, dayOfWeek, officeHours))
            .Should()
            .ThrowAsync<ValidationFailException>()
            .WithMessage(MessageResources.DoctorLicenseNotFound);  

        asserts.ShouldNotLogError();

        await asserts.ShouldNotStoreAsync(x => x.Id == doctorId);       
    }   

    [Fact(DisplayName = "A log should be written when fails to find a doctor")]
    public async Task SetOfficeHoursAsync_WhenFailsToFindDoctor_ThenThrowsException_AndLogsError() 
    {
        // Arrange    
        var doctorId = Guid.NewGuid();
        var officeHours = MorningHours.Select(TimeSpan.Parse).ToArray();

        var asserts = new DoctorsServiceAsserts();
        asserts.WhenFindAsyncThrows(doctorId);
        
        // Act & Assert
        var service = asserts.GetService();
        await service.Awaiting(x => x.SetOfficeHoursAsync(doctorId, DayOfWeek.Monday, officeHours))
            .Should()
            .ThrowAsync<ServiceFailException>()
            .WithMessage(MessageResources.DoctorSetFail);  

        asserts.ShouldLogError();

        await asserts.ShouldNotStoreAsync(x => x.Id == doctorId);       
    }
}