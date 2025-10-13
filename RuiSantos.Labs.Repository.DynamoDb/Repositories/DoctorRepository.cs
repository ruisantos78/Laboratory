using RuiSantos.Labs.Domain.Models;
using RuiSantos.Labs.Domain.Repositories;
using RuiSantos.Labs.Repository.DynamoDb.Commands;
using RuiSantos.Labs.Repository.DynamoDb.Commands.Doctors;

namespace RuiSantos.Labs.Repository.DynamoDb.Repositories;

internal class DoctorRepository(IServiceProvider provider): IDoctorRepository
{
    private readonly DoctorsCommandFactory _doctorsCommandFactory = new(provider);
    
    public async Task<Doctor?> FindAsync(Guid id, CancellationToken cancellationToken)
    {
        var command = _doctorsCommandFactory.CreateGetDoctorByIdCommand(id);
        return await command.ExecuteAsync(cancellationToken);
    }

    public async Task<Pagination<Doctor>> FindAllAsync(int take, string? paginationToken, CancellationToken cancellationToken)
    {
        var command = _doctorsCommandFactory.CreateGetDoctorsPaginationCommand(new GetDoctorsPaginationCommandInput
        {
            Take   = take,
            PaginationToken = paginationToken
        });

        return await command.ExecuteAsync(cancellationToken);
    }

    public Task<IEnumerable<Doctor>> FindBySpecialityAsync(string specialty, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Doctor>> FindByAppointmentsAsync(IEnumerable<Appointment> appointments, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<DoctorSchedule> FindBySpecialtyWithAvailabilityAsync(string specialty, DateOnly date,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Doctor?> FindByLicenseAsync(string license, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task StoreAsync(Doctor doctor, CancellationToken cancellationToken)
    {
        var command = _doctorsCommandFactory.CreateStoreDoctorCommand(doctor);
        await command.ExecuteAsync(cancellationToken);
    }
}