using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using RuiSantos.Labs.Core;
using RuiSantos.Labs.Core.Resources;
using RuiSantos.Labs.Core.Services.Exceptions;
using RuiSantos.Labs.Tests.Asserts;
using RuiSantos.Labs.Tests.Asserts.Builders;

namespace RuiSantos.Labs.Tests.Stories.Doctor;

// As a doctor, I want to be able to define and manage my work hours.
public class DoctorOfficeHoursManagement
{
    private static readonly string[] MorningHours = new[] { "09:00", "09:30", "10:00", "10:30", "11:00", "11:30" };
    private static readonly string[] AfternoonHours = new[] { "13:00", "13:30", "14:00", "14:30", "15:00", "15:30" };

    public static IEnumerable<object[]> OfficeHours => new List<object[]> {
        new object[] { "ABC123", DayOfWeek.Monday, MorningHours },
        new object[] { "ABC123", DayOfWeek.Wednesday, MorningHours },
        new object[] { "ABC123", DayOfWeek.Friday, MorningHours },
    };

    [Theory(DisplayName = "The doctor should be able the set with hours they attend by week days")]
    [MemberData(nameof(OfficeHours))]
    public async Task SetOfficeHoursAsync_WithSuccess(string license, DayOfWeek dayOfWeek, string[] hours)
    {
        // Arrange    
        var timespans = hours.Select(TimeSpan.Parse).ToArray();

        var asserts = new DoctorsAsserts();

        asserts.DoctorRepository.FindAsync(license).Returns(new DoctorBuilder()
            .With(license)
            .Build()
        );
        
        // Act
        var service = asserts.GetService();
        await service.SetOfficeHoursAsync(license, dayOfWeek, timespans);

        // Assert
        await asserts.DoctorRepository.Received(1)
            .StoreAsync(Arg.Is<Core.Models.Doctor>(x => 
                x.License == license && 
                x.OfficeHours.All(y => y.Week == dayOfWeek && y.Hours.SequenceEqual(timespans))
            ));

        asserts.Logger.DidNotReceiveWithAnyArgs().Fail(default);
    }

    [Theory(DisplayName = "The doctor should be able to modify their weekly schedule by changing the office hours.")]
    [MemberData(nameof(OfficeHours))]
    public async Task SetOfficeHoursAsync_WhenHasOfficeHours_ShouldReplace_WithSuccess(string license, DayOfWeek dayOfWeek, string[] hours)
    {
        // Arrange    
        var asserts = new DoctorsAsserts();

        asserts.DoctorRepository.FindAsync(license).Returns(new DoctorBuilder()
            .With(license)
            .WithOfficeHours(DayOfWeek.Monday, AfternoonHours)
            .WithOfficeHours(DayOfWeek.Wednesday, AfternoonHours)
            .WithOfficeHours(DayOfWeek.Friday, AfternoonHours)
            .Build()
        );
        
        var timespans = hours.Select(TimeSpan.Parse).ToArray();
        var afternoon = AfternoonHours.Select(TimeSpan.Parse).ToArray();

        // Act
        var service = asserts.GetService();
        await service.SetOfficeHoursAsync(license, dayOfWeek, timespans);

        // Assert
        await asserts.DoctorRepository.Received(1)
            .StoreAsync(Arg.Is<Core.Models.Doctor>(x => 
                x.License == license && 
                x.OfficeHours.All(y => 
                    (y.Week == dayOfWeek && y.Hours.SequenceEqual(timespans)) ||
                    (y.Week != dayOfWeek && y.Hours.SequenceEqual(afternoon))
                ))
            );

        asserts.Logger.DidNotReceiveWithAnyArgs().Fail(default);
    }

    [Theory(DisplayName = "It should not be possible to establish office hours for a doctor who is not registered.")]    
    [MemberData(nameof(OfficeHours))]
    public async Task SetOfficeHoursAsync_WhenDoctorDoesNotExist_ThrownValidationFailException(string license, DayOfWeek dayOfWeek, string[] hours)
    {
        // Arrange    
        var asserts = new DoctorsAsserts();

        asserts.DoctorRepository.FindAsync(license).ReturnsNull();
        var timespans = hours.Select(TimeSpan.Parse).ToArray();

        // Act & Assert
        var service = asserts.GetService();

        await service.Awaiting(x => x.SetOfficeHoursAsync(license, dayOfWeek, timespans))
            .Should()
            .ThrowAsync<ValidationFailException>()
            .WithMessage(MessageResources.DoctorLicenseNotFound); 

        await asserts.DoctorRepository.DidNotReceiveWithAnyArgs()
            .StoreAsync(Arg.Any<Core.Models.Doctor>());

        asserts.Logger.DidNotReceiveWithAnyArgs().Fail(default);
    }    
}