using System.Runtime.CompilerServices;
using RuiSantos.Labs.Core.Repositories;
using RuiSantos.Labs.Core.Models;
using RuiSantos.Labs.Data.Dynamodb.Adapters;
using RuiSantos.Labs.Domain.Extensions.Models;
using RuiSantos.Labs.Domain.Extensions.Repositories;

namespace RuiSantos.Labs.Data.Dynamodb.Repositories;

public class DoctorRepository(
    IDoctorAdapter doctorAdapter,
    IAppointmentAdapter appointmentAdapter)
    : IDoctorRepository
{
    public Task<Doctor?> FindAsync(Guid doctorId, CancellationToken cancellationToken)
    {
        return doctorAdapter.FindAsync(doctorId, cancellationToken);
    }

    public Task<IEnumerable<Doctor>> FindBySpecialityAsync(string specialty, CancellationToken cancellationToken)
    {
        return doctorAdapter.LoadBySpecialtyAsync(specialty, cancellationToken);
    }

    public async IAsyncEnumerable<DoctorSchedule> FindBySpecialtyWithAvailabilityAsync(string specialty, DateOnly date
        ,[EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach (var doctor in await doctorAdapter.LoadBySpecialtyAsync(specialty, cancellationToken))
        {
            if (doctor.OfficeHours.All(h => h.Week != date.DayOfWeek))
                continue;

            foreach (var appointment in await appointmentAdapter.LoadByDoctorAsync(doctor, date, cancellationToken))
            {
                var availableTimes = doctor.OfficeHours
                    .Where(x => x.Week == date.DayOfWeek)
                    .SelectMany(s => s.Hours.Where(hour => hour != appointment.Time))
                    .Select(x => date.ToDateTime(TimeOnly.FromTimeSpan(x)))
                    .ToList();

                if (availableTimes.Count != 0)
                    yield return new DoctorSchedule(doctor, availableTimes);
            }
        }
    }

    public async Task<IEnumerable<Doctor>> FindByAppointmentsAsync(IEnumerable<Appointment> appointments, CancellationToken cancellationToken)
    {
        var tasks = appointments.Select(x => doctorAdapter.FindByAppointmentAsync(x, cancellationToken))
            .OfType<Task<Doctor>>();

        return await Task.WhenAll(tasks);
    }

    public Task StoreAsync(Doctor doctor, CancellationToken cancellationToken)
    {
        return doctorAdapter.StoreAsync(doctor, cancellationToken);
    }

    public Task<Pagination<Doctor>> FindAllAsync(int take, string? paginationToken, CancellationToken cancellationToken)
    {
        return doctorAdapter.LoadByLicenseAsync(take, paginationToken, cancellationToken);
    }

    public Task<Doctor?> FindByLicenseAsync(string license, CancellationToken cancellationToken)
    {
        return doctorAdapter.FindByLicenseAsync(license, cancellationToken);
    }
}