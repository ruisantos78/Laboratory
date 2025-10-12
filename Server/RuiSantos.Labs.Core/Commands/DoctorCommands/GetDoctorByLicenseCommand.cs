using Microsoft.Extensions.Logging;
using RuiSantos.Labs.Core.Extensions;
using RuiSantos.Labs.Core.Models;
using RuiSantos.Labs.Core.Repositories;
using RuiSantos.Labs.Core.Resources;
using RuiSantos.Labs.Core.Services.Exceptions;

namespace RuiSantos.Labs.Core.Commands.DoctorCommands;

internal class GetDoctorByLicenseCommand(GetDoctorByLicenseCommandInput input
    , IDoctorRepository doctorRepository
    , ILogger<GetDoctorByLicenseCommand> logger)
    : IAsyncCommand<Doctor?>
{
    public async Task<Doctor?> Execute(CancellationToken cancellationToken = default)
    {
        try
        {
            return await doctorRepository.FindByLicenseAsync(input.License, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.Fail(ex);
            throw new ServiceFailException(MessageResources.DoctorsGetFail);
        }
    }
}

internal class GetDoctorByLicenseCommandInput
{
    public required string License { get; init; }
}