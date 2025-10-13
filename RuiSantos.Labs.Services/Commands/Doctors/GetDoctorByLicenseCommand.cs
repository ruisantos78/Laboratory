using FluentValidation;
using RuiSantos.Labs.Domain.Commands;
using RuiSantos.Labs.Domain.Repositories;
using RuiSantos.Labs.Services.Endpoints.Converters;
using RuiSantos.Labs.Services.Endpoints.Schemas;

namespace RuiSantos.Labs.Services.Commands.Doctors;

internal class GetDoctorByLicenseCommand(GetDoctorByLicenseCommandInput input
    , IDoctorRepository repository
    , ILogger<GetDoctorByLicenseCommand> logger
    ): IAsyncCommand<GetDoctorResponse?>
{
    public async Task<GetDoctorResponse?> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Executing GetDoctorByLicenseCommand");
        try
        {
            await ValidateAndThrowAsync(cancellationToken);
            var doctor = await repository.FindByLicenseAsync(input.License, cancellationToken);
            return doctor?.ToGetDoctorResponse();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error executing GetDoctorByLicenseCommand");
            throw;
        }
    }

    private async Task ValidateAndThrowAsync(CancellationToken cancellationToken)
    {
        var validator = new InlineValidator<GetDoctorByLicenseCommandInput>();

        validator.RuleFor(x => x.License)
            .NotEmpty()
            .WithMessage("License must not be empty.");
        
        await validator.ValidateAndThrowAsync(input, cancellationToken: cancellationToken);
    }
}

internal record GetDoctorByLicenseCommandInput
{
    public required string License { get; init; }
}