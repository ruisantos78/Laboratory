using FluentValidation;
using RuiSantos.Labs.Domain.Models;

namespace RuiSantos.Labs.Core.Validators;

internal sealed class DoctorValidator : AbstractValidator<Doctor>
{
    public DoctorValidator()
    {
        RuleFor(model => model)
            .NotNull();

        RuleFor(model => model.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(model => model.FirstName)
            .NotEmpty();

        RuleFor(model => model.LastName)
            .NotEmpty();

        RuleFor(model => model.License)
            .NotEmpty();

        RuleFor(model => model.Specializations)
            .NotEmpty();

        RuleForEach(model => model.Specializations)
            .NotEmpty();
    }
}