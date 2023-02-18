using System;
using System.Numerics;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using RuiSantos.ZocDoc.Core.Data;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Core.Validators;

namespace RuiSantos.ZocDoc.Core.Managers;

public abstract class ManagementBase
{
    protected readonly IDataContext context;
    protected readonly ILogger? logger;

    public ManagementBase(IDataContext context)
    {
        this.context = context;
        this.logger = null;
    }

    public ManagementBase(IDataContext context, ILogger logger) : this(context)
    {
        this.logger = logger;
    }

    public static bool IsValid<TValidator, TModel>(TModel model, TValidator validator, out ValidationFailException exception)
        where TValidator : AbstractValidator<TModel>
    {
        var validation = validator.Validate(model);
        if (!validation.IsValid)
        {
            exception = new ValidationFailException(validation.Errors);
            return false;
        }

        exception = ValidationFailException.Empty;
        return true;
    }
}

