using NSubstitute;
using RuiSantos.Labs.Core.Repositories;
using RuiSantos.Labs.Core.Services;

namespace RuiSantos.Labs.Tests.Asserts;

internal class MedicalSpecialtiesAsserts: ServiceAsserts<MedicalSpecialtiesService>
{
    private readonly IMedicalSpecialityRepository _repository = Substitute.For<IMedicalSpecialityRepository>();

    public IMedicalSpecialtiesService GetService() => new MedicalSpecialtiesService(_repository, Logger);

    public Task ShouldAddAsync(IEnumerable<string> specialties)
    {
        return _repository.Received()
            .AddAsync(specialties);
    }

    public Task ShouldRemoveAsync(string specialty)
    {
        return _repository.Received()
            .RemoveAsync(specialty);
    }

    public void WhenAddAsyncThrows(IEnumerable<string> specialties)
    {
        _repository.When(x => x.AddAsync(specialties))
            .Throw<Exception>();
    }

    public void WhenRemoveAsyncThrows(string specialty)
    {
        _repository.When(x => x.RemoveAsync(specialty))
            .Throw<Exception>();
    }
}