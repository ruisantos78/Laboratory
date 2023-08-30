﻿using FluentValidation;
using FluentValidation.Validators;
using RuiSantos.Labs.Core.Models;
using System.Text.RegularExpressions;

namespace RuiSantos.Labs.Core.Validators;

internal sealed class PatientValidator : AbstractValidator<Patient>
{
    private readonly Regex SocialSecurityNumberRegex = new(@"^(?!666|000|9\d{2})\d{3}-(?!00)\d{2}-(?!0{4})\d{4}$", RegexOptions.Compiled, TimeSpan.FromSeconds(5));

    public PatientValidator()
    {
        RuleFor(model => model.Id)
            .NotNull();

        RuleFor(model => model.SocialSecurityNumber)
            .NotEmpty()
            .Matches(SocialSecurityNumberRegex);

        RuleFor(model => model.Email)
            .NotEmpty()
            .EmailAddress(EmailValidationMode.AspNetCoreCompatible);

        RuleFor(model => model.FirstName)
            .NotEmpty();

        RuleFor(model => model.LastName)
            .NotEmpty();
    }
}
