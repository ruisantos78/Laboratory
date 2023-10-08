using RuiSantos.Labs.Client.Core.Mediators;
using RuiSantos.Labs.Client.Models;
using StrawberryShake;

namespace RuiSantos.Labs.Client.Services;

[RegisterService(typeof(DoctorService))]
public interface IDoctorService
{
    Task<DoctorModel?> GetDoctorByLicenseAsync(string license);
    
    Task<PaginationModel<DoctorModel>> GetDoctorsAsync(int recordCount, string? paginationToken);

    Task SetDoctorAsync(string license, string firstName, string lastName, string email, IEnumerable<string> contacts,
        IEnumerable<string> specialties);
}

public class DoctorService : IDoctorService
{
    private readonly ILabsClient _client;

    public DoctorService(ILabsClient client)
    {
        _client = client;
    }

    public async Task<DoctorModel?> GetDoctorByLicenseAsync(string license)
    {
        var response = await _client.GetDoctor.ExecuteAsync(license);
        response.EnsureNoErrors();

        if (response.Data?.Doctor is not { } doctor)
            return default;

        return new DoctorModel
        {
            License = doctor.License,
            FirstName = doctor.FirstName,
            LastName = doctor.LastName,
            Email = doctor.Email,
            Contacts = doctor.Contacts,
            Specialties = doctor.Specialties
        };
    }

    public async Task<PaginationModel<DoctorModel>> GetDoctorsAsync(int recordCount, string? paginationToken)
    {
        var response = await _client.GetDoctors.ExecuteAsync(new()
        {
            Take = recordCount,
            Token = paginationToken
        });

        response.EnsureNoErrors();

        if (response.Data?.Doctors is null)
            return new();

        var doctors = response.Data.Doctors.Doctors.Select(x => new DoctorModel
        {
            License = x.License,
            FirstName = x.FirstName,
            LastName = x.LastName,
            Email = x.Email,
            Contacts = x.Contacts,
            Specialties = x.Specialties
        });

        return new(doctors, response.Data.Doctors.PaginationToken);
    }

    public async Task SetDoctorAsync(string license, string firstName, string lastName, string email,
        IEnumerable<string> contacts, IEnumerable<string> specialties)
    {
        var response = await _client.SetDoctor.ExecuteAsync(new SetDoctorInput
        {
            Doctor = new()
            {
                License = license,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Contacts = contacts.ToList(),
                Specialties = specialties.ToList()
            }
        });

        response.EnsureNoErrors();
    }
}