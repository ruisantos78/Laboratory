using RuiSantos.Labs.Client.Core.Mediators;
using StrawberryShake;

namespace RuiSantos.Labs.Client.Services;

[RegisterService(typeof(MedicalSpecialtiesService))]
public interface IMedicalSpecialtiesService
{
    Task<IReadOnlyList<string>> GetMedicalSpecialtiesAsync();
    Task<IReadOnlyList<string>> StoreMedicalSpecialtiesAsync(IEnumerable<string> specialties);
    Task<IReadOnlyList<string>> RemoveMedicalSpecialtyAsync(string specialty);
}

public class MedicalSpecialtiesService : IMedicalSpecialtiesService
{
    private readonly ILabsClient _client;

    public MedicalSpecialtiesService(ILabsClient client)
    {
        _client = client;
    }

    public async Task<IReadOnlyList<string>> GetMedicalSpecialtiesAsync()
    {
        var response = await _client.GetMedicalSpecialties.ExecuteAsync();
        response.EnsureNoErrors();

        return response.Data?.Specialties
            .Select(x => x.Description)
            .ToArray() ?? Array.Empty<string>();
    }

    public async Task<IReadOnlyList<string>> StoreMedicalSpecialtiesAsync(IEnumerable<string> specialties)
    {
        var response = await _client.AddSpecialties.ExecuteAsync(new AddSpecialtiesInput
        {
            Descriptions = specialties.ToArray()
        });

        response.EnsureNoErrors();

        return response.Data?.AddSpecialties.Specialties?.Select(x => x.Description).ToArray()
               ?? Array.Empty<string>();
    }

    public async Task<IReadOnlyList<string>> RemoveMedicalSpecialtyAsync(string specialty)
    {
        var response = await _client.RemoveSpecialties.ExecuteAsync(new RemoveSpecialtiesInput
        {
            Description = specialty
        });

        response.EnsureNoErrors();

        return response.Data?.RemoveSpecialties.Specialties?.Select(x => x.Description).ToArray()
               ?? Array.Empty<string>();
    }
}