using Microsoft.Extensions.Logging;
using RuiSantos.Labs.Core.Extensions;
using RuiSantos.Labs.Core.Resources;
using RuiSantos.Labs.Core.Services.Exceptions;
using RuiSantos.Labs.Core.Validators;
using RuiSantos.Labs.Domain.Commands;
using RuiSantos.Labs.Domain.Models;
using RuiSantos.Labs.Domain.Repositories;

namespace RuiSantos.Labs.Core.Commands.DoctorCommands;

internal class CreateDoctorCommand(CreateDoctorCommandInput input
    , IDoctorRepository repository
    , ILogger<CreateDoctorCommand> logger
    ): IAsyncCommand
{
    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var doctor = GetModel();
            Validator.ThrowExceptionIfIsNotValid(doctor);
            
            await repository.StoreAsync(doctor, cancellationToken);
        }
        catch (ValidationFailException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.Fail(ex);
            throw new ServiceFailException(MessageResources.DoctorSetFail);
        }
    }

    private Doctor GetModel()
    {
        return new Doctor()
        {
            License = input.License,
            Email = input.Email,
            FirstName = input.FirstName,
            LastName = input.LastName,
            ContactNumbers = input.ContactNumbers.ToHashSet(),
            Specializations = input.Specialties.ToHashSet()
        };
    }  
}

internal class CreateDoctorCommandInput
{
    public required string License { get; init; }
    public required string Email { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required IReadOnlyList<string> ContactNumbers { get; init; }
    public required IReadOnlyList<string> Specialties { get; init; }
}