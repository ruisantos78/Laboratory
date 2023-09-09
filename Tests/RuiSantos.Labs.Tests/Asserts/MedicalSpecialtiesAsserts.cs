using NSubstitute;
using RuiSantos.Labs.Core.Cache;
using RuiSantos.Labs.Core.Repositories;
using RuiSantos.Labs.Core.Services;

namespace RuiSantos.Labs.Tests.Asserts;

internal class MedicalSpecialtiesAsserts: ServiceAsserts<MedicalSpecialtiesService>
{
    private IMedicalSpecialityRepository Repository { get; }
    private IRepositoryCache Cache { get; }

    public MedicalSpecialtiesAsserts(): base()
    {
        Repository = Substitute.For<IMedicalSpecialityRepository>();
        Cache = Substitute.For<IRepositoryCache>();
    }

    public IMedicalSpecialtiesService GetService()
    {
        return new MedicalSpecialtiesService(
            Repository,
            Cache,
            Logger
        );
    }

    public void ShouldClearCache()
    {
        Cache.Received()
            .ClearMedicalSpecialties();
    }

    public void ShouldNotClearCache()
    {
        Cache.DidNotReceive()
            .ClearMedicalSpecialties();
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
