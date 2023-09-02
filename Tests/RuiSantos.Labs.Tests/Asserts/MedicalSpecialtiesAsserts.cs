using Microsoft.Extensions.Logging;
using NSubstitute;
using RuiSantos.Labs.Core.Cache;
using RuiSantos.Labs.Core.Repositories;
using RuiSantos.Labs.Core.Services;

namespace RuiSantos.Labs.Tests.Asserts;

internal class MedicalSpecialtiesAsserts
{
    public IMedicalSpecialityRepository Repository { get; }
    public IRepositoryCache Cache { get; }
    public ILogger<MedicalSpecialtiesService> Logger { get; }

    public MedicalSpecialtiesAsserts()
    {
        Repository = Substitute.For<IMedicalSpecialityRepository>();
        Cache = Substitute.For<IRepositoryCache>();
        Logger = Substitute.For<ILogger<MedicalSpecialtiesService>>();
    }

    public IMedicalSpecialtiesService GetService()
    {
        return new MedicalSpecialtiesService(
            Repository,
            Cache,
            Logger
        );
    }
}
