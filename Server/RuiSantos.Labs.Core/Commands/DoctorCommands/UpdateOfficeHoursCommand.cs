using Microsoft.Extensions.Logging;
using RuiSantos.Labs.Core.Extensions;
using RuiSantos.Labs.Core.Resources;
using RuiSantos.Labs.Core.Services.Exceptions;
using RuiSantos.Labs.Domain.Commands;
using RuiSantos.Labs.Domain.Models;
using RuiSantos.Labs.Domain.Repositories;

namespace RuiSantos.Labs.Core.Commands.DoctorCommands;

internal class UpdateOfficeHoursCommand(UpdateOfficeHoursCommandInput input
    , IDoctorRepository repository
    , ILogger<UpdateOfficeHoursCommand> logger
    ): IAsyncCommand
{
    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (await repository.FindAsync(input.DoctorId, cancellationToken) is not { } doctor)
                throw new ValidationFailException(MessageResources.DoctorLicenseNotFound);

            doctor.OfficeHours.RemoveWhere(hour => hour.Week == input.DayOfWeek);
            
            if (input.Hours.Any())
                doctor.OfficeHours.Add(new OfficeHour(input.DayOfWeek, input.Hours));

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
}

internal class UpdateOfficeHoursCommandInput
{
    public Guid DoctorId { get; init; }
    public DayOfWeek DayOfWeek { get; init; }
    public IReadOnlyList<TimeSpan> Hours { get; init; } = [];
}