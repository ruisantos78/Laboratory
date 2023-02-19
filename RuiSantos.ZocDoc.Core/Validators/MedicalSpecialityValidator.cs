using FluentValidation;
using RuiSantos.ZocDoc.Core.Models;

namespace RuiSantos.ZocDoc.Core.Validators;

internal class MedicalSpecialityValidator : AbstractValidator<MedicalSpeciality>
{
    public readonly static MedicalSpecialityValidator Instance = new();

    public MedicalSpecialityValidator()
    {
        RuleFor(model => model.Description).NotEmpty();
    }
}