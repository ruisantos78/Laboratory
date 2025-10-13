using RuiSantos.Labs.Domain.Commands;
using RuiSantos.Labs.Domain.Models;
using RuiSantos.Labs.Services.Commands.Doctors;

namespace RuiSantos.Labs.Services.Commands;

internal interface IDoctorsCommandFactory
{
    IAsyncCommand<Pagination<Doctor>> GetDoctorsCommand(GetDoctorsCommandInput input);
}

internal class DoctorsCommandFactory(IServiceProvider provider): IDoctorsCommandFactory
{
    public IAsyncCommand<Pagination<Doctor>> GetDoctorsCommand(GetDoctorsCommandInput input)
        => ActivatorUtilities.CreateInstance<GetDoctorsCommand>(provider, input);
}