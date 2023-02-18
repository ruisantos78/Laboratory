using FluentValidation;
using RuiSantos.ZocDoc.Core.Models;

namespace RuiSantos.ZocDoc.Core.Validators;

internal class ScheduleValidator : AbstractValidator<Schedule>
{
    public readonly static ScheduleValidator Instance = new();

    public ScheduleValidator()
    {
        RuleFor(model => model.DoctorId)
            .NotEmpty();

        RuleFor(model => model.OfficeHours)
            .NotNull();

        RuleFor(model => model.Appointments)
            .NotNull();
    }
}
