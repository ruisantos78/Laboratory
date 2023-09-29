using FluentAssertions;
using RuiSantos.Labs.Core.Resources;
using RuiSantos.Labs.Core.Services.Exceptions;
using RuiSantos.Labs.Tests.Asserts;
using RuiSantos.Labs.Tests.Asserts.Builders;

namespace RuiSantos.Labs.Tests.Stories.Doctor;

/// <UserStory>
/// As a doctor, I want to be able to define and manage my work hours.
/// </UserStory>
public class DoctorOfficeHoursManagement
{
    private static readonly string[] MorningHours = new[] { "09:00", "09:30", "10:00", "10:30", "11:00", "11:30" };
    private static readonly string[] AfternoonHours = new[] { "13:00", "13:30", "14:00", "14:30", "15:00", "15:30" };

    public static IEnumerable<object[]> OfficeHours => new List<object[]> {
        new object[] { DayOfWeek.Monday, MorningHours },
        new object[] { DayOfWeek.Wednesday, MorningHours },
        new object[] { DayOfWeek.Friday, MorningHours },
    };

    [Theory(DisplayName = "The doctor should be able the set with hours they attend by week days")]
    [MemberData(nameof(OfficeHours))]
    public async Task SetOfficeHoursAsync_WithSuccess(DayOfWeek dayOfWeek, string[] hours)
    {
        // Arrange
        var doctorId = Guid.NewGuid();
        var timespans = hours.Select(TimeSpan.Parse).ToArray();

        var asserts = new DoctorsAsserts();

        asserts.ReturnsOnFindAsync(doctorId, new DoctorBuilder(doctorId).Build());
        
        // Act
        var service = asserts.GetService();
        await service.SetOfficeHoursAsync(doctorId, dayOfWeek, timespans);

        // Assert
        asserts.ShouldNotLogError();

        await asserts.ShouldStoreAsync(x =>
            x.Id == doctorId &&
            x.OfficeHours.All(y => y.Week == dayOfWeek && y.Hours.SequenceEqual(timespans))); 
    }

    [Theory(DisplayName = "The doctor should be able to modify their weekly schedule by changing the office hours.")]
    [MemberData(nameof(OfficeHours))]
    public async Task SetOfficeHoursAsync_WhenHasOfficeHours_ShouldReplace_WithSuccess(DayOfWeek dayOfWeek, string[] hours)
    {
        // Arrange
        var doctorId = Guid.NewGuid();
        var asserts = new DoctorsAsserts();

        asserts.ReturnsOnFindAsync(doctorId, new DoctorBuilder(doctorId)
            .WithOfficeHours(DayOfWeek.Monday, AfternoonHours)
            .WithOfficeHours(DayOfWeek.Wednesday, AfternoonHours)
            .WithOfficeHours(DayOfWeek.Friday, AfternoonHours)
            .Build()
        );
        
        var timespans = hours.Select(TimeSpan.Parse).ToArray();
        var afternoon = AfternoonHours.Select(TimeSpan.Parse).ToArray();

        // Act
        var service = asserts.GetService();
        await service.SetOfficeHoursAsync(doctorId, dayOfWeek, timespans);

        // Assert
        asserts.ShouldNotLogError();

        await asserts.ShouldStoreAsync(x => 
            x.Id == doctorId && 
            x.OfficeHours.All(y => 
                (y.Week == dayOfWeek && y.Hours.SequenceEqual(timespans)) ||
                (y.Week != dayOfWeek && y.Hours.SequenceEqual(afternoon))
            ));
    }

    [Theory(DisplayName = "It should not be possible to establish office hours for a doctor who is not registered.")]    
    [MemberData(nameof(OfficeHours))]
    public async Task SetOfficeHoursAsync_WhenDoctorDoesNotExist_ThrownValidationFailException(DayOfWeek dayOfWeek, string[] hours)
    {
        // Arrange
        var doctorId = Guid.NewGuid();
        var asserts = new DoctorsAsserts();

        asserts.ReturnsOnFindAsync(doctorId, default);

        var timespans = hours.Select(TimeSpan.Parse).ToArray();

        // Act & Assert
        var service = asserts.GetService();
        await service.Awaiting(x => x.SetOfficeHoursAsync(doctorId, dayOfWeek, timespans))
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
        var timespans = MorningHours.Select(TimeSpan.Parse).ToArray();

        var asserts = new DoctorsAsserts();
        asserts.ThrowOnFindAsync(doctorId);
        
        // Act & Assert
        var service = asserts.GetService();
        await service.Awaiting(x => x.SetOfficeHoursAsync(doctorId, DayOfWeek.Monday, timespans))
            .Should()
            .ThrowAsync<ServiceFailException>()
            .WithMessage(MessageResources.DoctorSetFail);  

        asserts.ShouldLogError();

        await asserts.ShouldNotStoreAsync(x => x.Id == doctorId);       
    }
}