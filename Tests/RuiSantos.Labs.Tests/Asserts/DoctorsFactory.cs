using Microsoft.Extensions.Logging;
using NSubstitute;
using RuiSantos.Labs.Core.Cache;
using RuiSantos.Labs.Core.Repositories;
using RuiSantos.Labs.Core.Services;

namespace RuiSantos.Labs.Tests.Asserts;

internal class DoctorsFactory {
    public IRepositoryCache Cache { get; }
    public IDoctorRepository DoctorRepository { get; }
    public IPatientRepository PatientRepository { get; }
    public IAppointamentsRepository AppointamentsRepository { get; }
    public ILogger<DoctorService> Logger { get; }

    public DoctorsFactory()
    {
        Cache = Substitute.For<IRepositoryCache>();
        DoctorRepository = Substitute.For<IDoctorRepository>();
        PatientRepository = Substitute.For<IPatientRepository>();
        AppointamentsRepository = Substitute.For<IAppointamentsRepository>();
        Logger = Substitute.For<ILogger<DoctorService>>();
    }
    
    public IDoctorService CreateService()
    {
        return new DoctorService(
            Cache,
            DoctorRepository,
            PatientRepository,
            AppointamentsRepository,
            Logger);
    }
}