using FluentValidation;
using RuiSantos.Labs.Domain.Commands;
using RuiSantos.Labs.Domain.Repositories;
using RuiSantos.Labs.Services.Endpoints.Converters;
using RuiSantos.Labs.Services.Endpoints.Schemas;

namespace RuiSantos.Labs.Services.Commands.Doctors;

internal class StoreDoctorCommand(StoreDoctorRequest request
    , IDoctorRepository doctorRepository
    , ILogger<StoreDoctorCommand> logger
): IAsyncCommand
{
    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Storing doctor with {License}", request.LicenseNumber);
        try
        {
            await ValidateAndThrowAsync(cancellationToken);
            var doctor = request.ToDoctor();
            await doctorRepository.StoreAsync(doctor, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error storing doctor with {License}", request.LicenseNumber);
            throw;
        }
    }
    
    private async Task ValidateAndThrowAsync(CancellationToken cancellationToken)
    {
        var validator = new InlineValidator<StoreDoctorRequest>();

        validator.RuleFor(x => x.LicenseNumber).NotEmpty();
        validator.RuleFor(x => x.Email).NotEmpty().EmailAddress();
        validator.RuleFor(x => x.FirstName).NotEmpty();
        validator.RuleFor(x => x.LastName).NotEmpty();

        validator.RuleFor(x => x.Specializations).NotEmpty();
        validator.RuleForEach(x => x.Specializations).NotEmpty();
        validator.RuleFor(x => x.Specializations)
            .Must(x => x!.CountBy(y => y).All(z => z.Value == 1))
            .WithMessage("Specializations must be unique.");       

        validator.RuleFor(x => x.ContactNumbers).NotEmpty();
        validator.RuleForEach(x => x.ContactNumbers).NotEmpty();
        validator.RuleFor(x => x.ContactNumbers)
            .Must(x => x!.CountBy(y => y).All(z => z.Value == 1))
            .WithMessage("Contact numbers must be unique."); 

        await validator.ValidateAndThrowAsync(request, cancellationToken: cancellationToken);
    }
}
