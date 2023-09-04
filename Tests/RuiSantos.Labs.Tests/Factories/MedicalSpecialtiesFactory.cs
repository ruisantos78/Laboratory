using Microsoft.Extensions.Logging;
using NSubstitute;
using RuiSantos.Labs.Core.Cache;
using RuiSantos.Labs.Core.Repositories;
using RuiSantos.Labs.Core.Services;

namespace RuiSantos.Labs.Tests.Factories;

internal class MedicalSpecialtiesFactory
{
    public IMedicalSpecialityRepository Repository { get; }
    public IRepositoryCache Cache { get; }
    public ILogger<MedicalSpecialtiesService> Logger { get; }

    public MedicalSpecialtiesFactory()
    {
        Repository = Substitute.For<IMedicalSpecialityRepository>();
        Cache = Substitute.For<IRepositoryCache>();
        Logger = Substitute.For<ILogger<MedicalSpecialtiesService>>();
    }

    public IMedicalSpecialtiesService CreateService()
    {
        return new MedicalSpecialtiesService(
            Repository,
            Cache,
            Logger
        );    
    }    
}
