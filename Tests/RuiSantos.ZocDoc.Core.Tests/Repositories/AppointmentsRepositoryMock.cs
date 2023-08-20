using RuiSantos.ZocDoc.Core.Repositories;

namespace RuiSantos.ZocDoc.Core.Tests.Repositories;

public class AppointmentsRepositoryMock
{
    private readonly Mock<IAppointamentsRepository> repository;

    public IAppointamentsRepository Object => repository.Object;

    public AppointmentsRepositoryMock()
	{
        this.repository = new Mock<IAppointamentsRepository>();
    }
    
    public void ShouldStoreAsync(string socialNumber, string medicalLicence, DateTime dateTime, Func<Times> times)
    {
        repository.Verify(m => m.StoreAsync(
                It.Is<Doctor>(d => d.License == medicalLicence),
                It.Is<Patient>(p => p.SocialSecurityNumber == socialNumber),
                It.Is<Appointment>(a => a.GetDateTime() == dateTime)
            ), times);
    }

    public void ShouldRemoveAsync(string socialNumber, string medicalLicence, DateTime dateTime, Func<Times> times)
    {
        repository.Verify(m => m.RemoveAsync(
                It.Is<Doctor>(d => d.License == medicalLicence),
                It.Is<Patient>(p => p.SocialSecurityNumber == socialNumber),
                It.Is<Appointment>(a => a.GetDateTime() == dateTime)
            ), times);
    }
}

