using FluentAssertions;
using NSubstitute;
using RuiSantos.Labs.Core;
using RuiSantos.Labs.Core.Resources;
using RuiSantos.Labs.Core.Services.Exceptions;
using RuiSantos.Labs.Tests.Asserts;
using RuiSantos.Labs.Tests.Asserts.Builders;

using TDoctor = RuiSantos.Labs.Core.Models.Doctor;

namespace RuiSantos.Labs.Tests.Stories.Doctor;

// As a doctor, I want to be able to define and manage my work hours.
public class DoctorOfficeHoursManagement
{
    public static IEnumerable<object[]> OfficeHours => new List<object[]> {
        new object[] {
            "ABC123",
            DayOfWeek.Monday,
            new[] { "09:00", "09:30", "10:00", "10:30", "11:00", "11:30" }
        },
        new object[] {
            "ABC123",
            DayOfWeek.Wednesday,
            new[] { "09:00", "09:30", "10:00", "10:30", "11:00", "11:30" }
        },
        new object[] {
            "ABC123",
            DayOfWeek.Friday,
            new[] { "09:00", "09:30", "10:00", "10:30", "11:00", "11:30" }
        }
    };

    [Theory(DisplayName = "The doctor should be able the set with hours they attend by week days")]
    [MemberData(nameof(OfficeHours))]
    public async Task ShouldBeAbleToSetHoursByWeekDays_WithSuccess(string license, DayOfWeek dayOfWeek, string[] hours)
    {
        // Arrange    
        var timespans = hours.Select(x => TimeSpan.Parse(x));

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
            .StoreAsync(Arg.Is<TDoctor>(x => 
                x.License == license && 
                x.OfficeHours.All(y => y.Week == dayOfWeek && y.Hours.SequenceEqual(timespans))
            ));

        asserts.Logger.DidNotReceiveWithAnyArgs().Fail(default);
    }

    [Theory(DisplayName = "The doctor should be able to replace the office hours by week days")]
    [MemberData(nameof(OfficeHours))]
    public async Task ShouldBeAbleToReplaceOfficeHoursByWeekDays_WithSuccess(string license, DayOfWeek dayOfWeek, string[] hours)
    {
        // Arrange    
        var asserts = new DoctorsAsserts();

        asserts.DoctorRepository.FindAsync(license).Returns(new DoctorBuilder()
            .With(license)
            .WithOfficeHours(DayOfWeek.Monday, "13:00", "13:30", "14:00", "14:30", "15:00", "15:30")
            .WithOfficeHours(DayOfWeek.Wednesday, "13:00", "13:30", "14:00", "14:30", "15:00", "15:30")
            .WithOfficeHours(DayOfWeek.Friday,  "13:00", "13:30", "14:00", "14:30", "15:00", "15:30")
            .Build()
        );
        
        var timespans = hours.Select(x => TimeSpan.Parse(x));

        // Act
        var service = asserts.GetService();
        await service.SetOfficeHoursAsync(license, dayOfWeek, timespans);

        // Assert
        await asserts.DoctorRepository.Received(1)
            .StoreAsync(Arg.Is<TDoctor>(x => 
                x.License == license && 
                x.OfficeHours.All(y => 
                    (y.Week == dayOfWeek && y.Hours.SequenceEqual(timespans)) ||
                    (y.Week != dayOfWeek && y.Hours.Min().Hours == 13 && y.Hours.Max().Hours == 15)
                ))
            );

        asserts.Logger.DidNotReceiveWithAnyArgs().Fail(default);
    }

    [Theory(DisplayName = "The doctor should not be able to set office hours by week days if the doctor does not exist")]    
    [MemberData(nameof(OfficeHours))]
    public async Task ShouldNotBeAbleToSetOfficeHoursByWeekDays_IfDoctorDoesNotExist_WithSuccess(string license, DayOfWeek dayOfWeek, string[] hours)
    {
        // Arrange    
        var asserts = new DoctorsAsserts();

        asserts.DoctorRepository.FindAsync(license).Returns(default(TDoctor));
        var timespans = hours.Select(x => TimeSpan.Parse(x));

        // Act & Assert
        var service = asserts.GetService();

        await service.Awaiting(x => x.SetOfficeHoursAsync(license, dayOfWeek, timespans))
            .Should()
            .ThrowAsync<ValidationFailException>()
            .WithMessage(MessageResources.DoctorLicenseNotFound); 

        await asserts.DoctorRepository.DidNotReceiveWithAnyArgs()
            .StoreAsync(Arg.Any<TDoctor>());

        asserts.Logger.DidNotReceiveWithAnyArgs().Fail(default);
    }    
}