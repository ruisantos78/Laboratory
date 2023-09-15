using NSubstitute;
using RuiSantos.Labs.Core.Repositories;
using RuiSantos.Labs.Core.Services;

namespace RuiSantos.Labs.Tests.Asserts;

internal class MedicalSpecialtiesAsserts: ServiceAsserts<MedicalSpecialtiesService>
{
    private IMedicalSpecialityRepository Repository { get; }

    public MedicalSpecialtiesAsserts(): base()
    {
        Repository = Substitute.For<IMedicalSpecialityRepository>();
    }

    public IMedicalSpecialtiesService GetService()
    {
        return new MedicalSpecialtiesService(
            Repository,
            Logger
        );
    }

    public async Task ShouldAddAsync(IEnumerable<string> specialties)
    {
        await Repository.Received()
            .AddAsync(specialties);
    }

    public async Task ShouldRemoveAsync(string specialty)
    {
        await Repository.Received()
            .RemoveAsync(specialty);
    }

    public void ThrowsOnAddAsync(IEnumerable<string> specialties)
    {
        Repository.When(x => x.AddAsync(specialties))
            .Throw<Exception>();
    }

    public void ThrowsOnRemoveAsync(string specialty)
    {
        Repository.When(x => x.RemoveAsync(specialty))
            .Throw<Exception>();
    }
}
