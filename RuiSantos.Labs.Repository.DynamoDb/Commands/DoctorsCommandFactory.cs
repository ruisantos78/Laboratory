using Microsoft.Extensions.DependencyInjection;
using RuiSantos.Labs.Domain.Commands;
using RuiSantos.Labs.Domain.Models;
using RuiSantos.Labs.Repository.DynamoDb.Commands.Doctors;

namespace RuiSantos.Labs.Repository.DynamoDb.Commands;

internal class DoctorsCommandFactory(IServiceProvider provider)
{
    public IAsyncCommand<Doctor?> CreateGetDoctorByIdCommand(Guid doctorId)
        => ActivatorUtilities.CreateInstance<GetDoctorByIdCommand>(provider, doctorId);
        
    public IAsyncCommand<Pagination<Doctor>> CreateGetDoctorsPaginationCommand(GetDoctorsPaginationCommandInput input)
        => ActivatorUtilities.CreateInstance<GetDoctorsPaginationCommand>(provider, input);
    
    public IAsyncCommand CreateStoreDoctorCommand(Doctor doctor)
        => ActivatorUtilities.CreateInstance<StoreDoctorCommand>(provider, doctor);
}
