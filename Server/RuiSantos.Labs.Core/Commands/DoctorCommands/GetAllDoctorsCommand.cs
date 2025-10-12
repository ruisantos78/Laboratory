using Microsoft.Extensions.Logging;
using RuiSantos.Labs.Core.Extensions;
using RuiSantos.Labs.Core.Models;
using RuiSantos.Labs.Core.Repositories;
using RuiSantos.Labs.Core.Resources;
using RuiSantos.Labs.Core.Services.Exceptions;

namespace RuiSantos.Labs.Core.Commands.DoctorCommands;

internal class GetAllDoctorsCommand(GetAllDoctorsCommandInput input
    ,IDoctorRepository doctorRepository
    , ILogger<GetAllDoctorsCommand> logger
    ): IAsyncCommand<Pagination<Doctor>>
{
    public async Task<Pagination<Doctor>> Execute(CancellationToken cancellationToken = default)
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