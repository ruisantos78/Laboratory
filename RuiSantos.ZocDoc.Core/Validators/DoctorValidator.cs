using FluentValidation;
using FluentValidation.Validators;
using RuiSantos.ZocDoc.Core.Models;

namespace RuiSantos.ZocDoc.Core.Validators;

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
    public DoctorValidator(IEnumerable<MedicalSpeciality>? specialties) : this()
    {
        if (specialties is not null)
        {
            RuleForEach(model => model.Specialties)
                .Must(item => specialties.Any(s => string.Equals(s.Description, item, StringComparison.OrdinalIgnoreCase)));
        }
    }
}

