using RuiSantos.Labs.Core.Commands.DoctorCommands;
using RuiSantos.Labs.Core.Factories;
using RuiSantos.Labs.Domain.Models;

namespace RuiSantos.Labs.Core.Services;

internal class DoctorService(IServiceProvider provider) : IDoctorService
{
    private readonly DoctorCommandFactory _doctorCommandFactory = new(provider);
    
    public async Task CreateDoctorAsync(string license, string email, string firstName, string lastName,
        IEnumerable<string> contactNumbers, IEnumerable<string> specialties, 
        CancellationToken cancellationToken = default)
    {
        await _doctorCommandFactory.CreateDoctorCommand(new CreateDoctorCommandInput
        {
            License = license,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            ContactNumbers = [.. contactNumbers],
            Specialties = [.. specialties]
        }).ExecuteAsync(cancellationToken);
    }

    public async Task SetOfficeHoursAsync(Guid doctorId, DayOfWeek dayOfWeek, IEnumerable<TimeSpan> hours, 
        CancellationToken cancellationToken = default)
    {
        await _doctorCommandFactory.UpdateOfficeHoursCommand(new UpdateOfficeHoursCommandInput
        {
            DoctorId = doctorId,
            DayOfWeek = dayOfWeek,
            Hours = [.. hours]
        }).ExecuteAsync(cancellationToken);
    }

    public async Task<IEnumerable<PatientAppointment>> GetAppointmentsAsync(Guid doctorId, DateTime? dateTime,
        CancellationToken cancellationToken = default)
    {
        return await _doctorCommandFactory.GetDoctorAppointmentsByDateCommand(new GetDoctorAppointmentsByDateCommandInput
        {
            DoctorId = doctorId,
            Date = DateOnly.FromDateTime(dateTime ?? DateTime.Today)
        }).ExecuteAsync(cancellationToken);
    }

    public async Task<Pagination<Doctor>> GetAllDoctorsAsync(int take, string? paginationToken,
        CancellationToken cancellationToken = default)
    {
        return await _doctorCommandFactory.GetAllDoctorsCommand(new GetAllDoctorsCommandInput()
        {
            Take = take,
            PaginationToken = paginationToken
        }).ExecuteAsync(cancellationToken);
    }

    public async Task<Doctor?> GetDoctorByLicenseAsync(string license, CancellationToken cancellationToken = default)
    {
        return await _doctorCommandFactory.GetDoctorByLicenseCommand(new GetDoctorByLicenseCommandInput
        {
            License = license
        }).ExecuteAsync(cancellationToken);
    }
}
