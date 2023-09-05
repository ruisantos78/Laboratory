using NSubstitute;
using RuiSantos.Labs.Core.Cache;
using RuiSantos.Labs.Core.Repositories;
using RuiSantos.Labs.Core.Services;

namespace RuiSantos.Labs.Tests.Asserts;

internal class MedicalSpecialtiesAsserts: ServiceAsserts<MedicalSpecialtiesService>
{
    public IMedicalSpecialityRepository Repository { get; }

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

    public void ShouldClearCache() {
        Cache.Received().ClearMedicalSpecialties(); 
    }

    public void ShouldNotClearCache() {
        Cache.DidNotReceive().ClearMedicalSpecialties(); 
    }
}
