using Microsoft.Extensions.Logging;
using RuiSantos.Labs.Core.Extensions;
using RuiSantos.Labs.Core.Resources;
using RuiSantos.Labs.Core.Services.Exceptions;
using RuiSantos.Labs.Domain.Commands;
using RuiSantos.Labs.Domain.Models;
using RuiSantos.Labs.Domain.Repositories;

namespace RuiSantos.Labs.Core.Commands.DoctorCommands;

internal class GetAllDoctorsCommand(GetAllDoctorsCommandInput input
    ,IDoctorRepository doctorRepository
    , ILogger<GetAllDoctorsCommand> logger
    ): IAsyncCommand<Pagination<Doctor>>
{
    public async Task<Pagination<Doctor>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await doctorRepository.FindAllAsync(input.Take, input.PaginationToken, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.Fail(ex);
            throw new ServiceFailException(MessageResources.DoctorsGetAppointmentsFail);
        }
    }
}

internal class GetAllDoctorsCommandInput
{
    public int Take { get; init; }
    public string? PaginationToken { get; init; }
}