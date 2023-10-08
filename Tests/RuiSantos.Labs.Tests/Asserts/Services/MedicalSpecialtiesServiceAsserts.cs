using RuiSantos.Labs.Core.Services;
using RuiSantos.Labs.Tests.Asserts.Repositories;

namespace RuiSantos.Labs.Tests.Asserts.Services;

internal class MedicalSpecialtiesServiceAsserts : LoggerAsserts<MedicalSpecialtiesService>
{
    private readonly MedicalSpecialityRepositoryAsserts _repository = new();

    public IMedicalSpecialtiesService GetService() => new MedicalSpecialtiesService(
        _repository.GetRepository(),
        Logger
    );

    public Task ShouldAddAsync(IEnumerable<string> specialties, bool received = true)
    {
        return _repository.ShouldAddAsync(specialties, received);
    }

    public Task ShouldRemoveAsync(string specialty, bool received = true)
    {
        return _repository.ShouldRemoveAsync(specialty, received);
    }

    public void WhenAddAsyncThrows(IEnumerable<string> specialties)
    {
        _repository.WhenAddAsyncThrows(specialties, new Exception());
    }

    public void WhenRemoveAsyncThrows(string specialty)
    {
        _repository.WhenRemoveAsyncThrows(specialty, new Exception());
    }
}