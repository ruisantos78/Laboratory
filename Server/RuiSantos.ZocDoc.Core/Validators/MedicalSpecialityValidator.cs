using FluentValidation;
using RuiSantos.ZocDoc.Core.Models;

namespace RuiSantos.ZocDoc.Core.Validators;

internal sealed class MedicalSpecialityValidator : AbstractValidator<MedicalSpecialty>
{
    public MedicalSpecialityValidator()
    {
        RuleFor(model => model.Description).NotEmpty();
    }
}