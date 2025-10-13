using FluentValidation;
using System.Text.RegularExpressions;
using RuiSantos.Labs.Domain.Models;

namespace RuiSantos.Labs.Core.Validators;

internal sealed class PatientValidator : AbstractValidator<Patient>
{
    private readonly Regex _socialSecurityNumberRegex = new(@"^(?!666|000|9\d{2})\d{3}-(?!00)\d{2}-(?!0{4})\d{4}$",
        RegexOptions.Compiled, TimeSpan.FromSeconds(5));

    public PatientValidator()
    {
        RuleFor(model => model.Id)
            .NotNull();

        RuleFor(model => model.SocialSecurityNumber)
            .NotEmpty()
            .Matches(_socialSecurityNumberRegex);

        RuleFor(model => model.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(model => model.FirstName)
            .NotEmpty();

        RuleFor(model => model.LastName)
            .NotEmpty();
    }
}
