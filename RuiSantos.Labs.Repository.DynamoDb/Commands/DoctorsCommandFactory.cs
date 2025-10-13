using Microsoft.Extensions.DependencyInjection;
using RuiSantos.Labs.Domain.Extensions.Commands;
using RuiSantos.Labs.Domain.Extensions.Models;
using RuiSantos.Labs.Repository.DynamoDb.Commands.Doctors;

namespace RuiSantos.Labs.Repository.DynamoDb.Commands;

internal class DoctorsCommandFactory(IServiceProvider provider)
{
    public IAsyncCommand CreateStoreDoctorCommand(Doctor doctor)
        => ActivatorUtilities.CreateInstance<StoreDoctorCommand>(provider, doctor);
    
    public IAsyncCommand<Pagination<Doctor>> CreateGetDoctorsPaginationCommand(GetDoctorsPaginationCommandInput input)
        => ActivatorUtilities.CreateInstance<GetDoctorsPaginationCommand>(provider, input);
}