using FluentValidation;
using FluentValidation.Validators;
using RuiSantos.Labs.Core.Models;

namespace RuiSantos.Labs.Core.Validators;

internal sealed class DoctorValidator : AbstractValidator<Doctor>
{
    public DoctorValidator()
    {
        RuleFor(model => model)
            .NotNull();

        RuleFor(model => model.Email)
            .NotEmpty()
            .EmailAddress(EmailValidationMode.AspNetCoreCompatible);

        RuleFor(model => model.FirstName)
            .NotEmpty();

        RuleFor(model => model.LastName)
            .NotEmpty();

        RuleFor(model => model.License)
            .NotEmpty();

        RuleFor(model => model.Specialties)
            .NotEmpty();

        RuleForEach(model => model.Specialties)
            .NotEmpty();
    }
}

