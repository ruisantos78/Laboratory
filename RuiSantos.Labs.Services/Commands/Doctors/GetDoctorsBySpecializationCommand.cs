using FluentValidation;
using RuiSantos.Labs.Domain.Commands;
using RuiSantos.Labs.Domain.Repositories;
using RuiSantos.Labs.Services.Endpoints.Converters;
using RuiSantos.Labs.Services.Endpoints.Schemas;

namespace RuiSantos.Labs.Services.Commands.Doctors;

internal class GetDoctorsBySpecializationCommand(GetDoctorsBySpecializationCommandInput input
    , IDoctorRepository repository
    , ILogger<GetDoctorsBySpecializationCommand> logger
    ): IAsyncCommand<GetDoctorsCollectionResponse>
{
    public async Task<GetDoctorsCollectionResponse> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Executing GetDoctorsBySpecializationCommand");
        try
        {
            await ValidateAndThrowAsync(cancellationToken);
            
            var doctors = await repository.FindBySpecializationAsync(input.Specialization, cancellationToken);
            return doctors.ToGetDoctorsCollectionResponse();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error executing GetDoctorsBySpecializationCommand");
            throw;
        }
    }

    private async Task ValidateAndThrowAsync(CancellationToken cancellationToken)
    {
        var validator = new InlineValidator<GetDoctorsBySpecializationCommandInput>();

        validator.RuleFor(x => x.Specialization)
            .NotEmpty()
            .WithMessage("Specialization must not be empty.");
        
        await validator.ValidateAndThrowAsync(input, cancellationToken: cancellationToken);
    }
}

internal record GetDoctorsBySpecializationCommandInput
{
    public required string Specialization { get; init; }
}