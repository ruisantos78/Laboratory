using FluentValidation;
using RuiSantos.Labs.Domain.Commands;
using RuiSantos.Labs.Domain.Repositories;
using RuiSantos.Labs.Services.Endpoints.Converters;
using RuiSantos.Labs.Services.Endpoints.Schemas;

namespace RuiSantos.Labs.Services.Commands.Doctors;

internal class GetDoctorByIdCommand(GetDoctorByIdCommandInput input
    , IDoctorRepository repository
    , ILogger<GetDoctorByIdCommand> logger
    ): IAsyncCommand<GetDoctorResponse?>
{
    private Guid _doctorId;
    
    public async Task<GetDoctorResponse?> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Executing GetDoctorByIdCommand");
        try
        {
            await ValidateAndThrowAsync(cancellationToken);
            var doctor = await repository.FindAsync(_doctorId, cancellationToken);
            return doctor?.ToGetDoctorResponse();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error executing GetDoctorByIdCommand");
            throw;
        }
    }

    private async Task ValidateAndThrowAsync(CancellationToken cancellationToken)
    {
        var validator = new InlineValidator<GetDoctorByIdCommandInput>();

        validator.RuleFor(x => x.DoctorId)
            .Must(x => Guid.TryParse(x, out _doctorId))
            .WithMessage("Doctor Id must be a valid Guid.");
        
        await validator.ValidateAndThrowAsync(input, cancellationToken: cancellationToken);
    }
}

internal record GetDoctorByIdCommandInput
{
    public required string DoctorId { get; init; }
}