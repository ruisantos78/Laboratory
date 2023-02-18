using FluentValidation;
using FluentValidation.Validators;
using RuiSantos.ZocDoc.Core.Models;

namespace RuiSantos.ZocDoc.Core.Validators;

internal class DoctorValidator: AbstractValidator<Doctor>
{
    public readonly static DoctorValidator Instance = new DoctorValidator();

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

        RuleFor(model => model.Specialities)
            .NotEmpty();

        RuleForEach(model => model.Specialities)
            .NotEmpty();
    }
}

