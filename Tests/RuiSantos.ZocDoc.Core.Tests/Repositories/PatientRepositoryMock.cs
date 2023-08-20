using System.Linq.Expressions;
using RuiSantos.ZocDoc.Core.Repositories;

namespace RuiSantos.ZocDoc.Core.Tests.Repositories;

public class PatientRepositoryMock
{
    private readonly Mock<IPatientRepository> respository;

    public IPatientRepository Object => respository.Object;

    public PatientRepositoryMock()
    {
        this.respository = new Mock<IPatientRepository>();
    }
    
    public void SetFindAsyncReturns(Func<string, Patient?> returns)
    {
        respository.Setup(m => m.FindAsync(It.IsAny<string>()))
            .ReturnsAsync(returns);
    }

    public void SetFindAllWithAppointmentsAsyncReturns(List<Patient> returns)
    {
        respository.Setup(m => m.FindAllWithAppointmentsAsync(It.IsAny<IEnumerable<Appointment>>()))
            .ReturnsAsync(returns);
    }

    public void SetFindAllWithAppointmentsAsyncReturns(Func<IEnumerable<Appointment>, List<Patient>> returns)
    {
        respository.Setup(m => m.FindAllWithAppointmentsAsync(It.IsAny<IEnumerable<Appointment>>()))
            .ReturnsAsync(returns);
    }

    public void SetStoreAsyncCallback(Action<Patient> callback)
    {
        respository.Setup(m => m.StoreAsync(It.IsAny<Patient>()))
            .Callback<Patient>(callback);
    }

    public void ShouldStoreAsync(Expression<Func<Patient, bool>> match)
    {
        respository.Verify(m => m.StoreAsync(It.Is(match)), Times.Once);
    }

    public void ShouldStoreAsync(Expression<Func<Patient, bool>> match, Func<Times> times)
    {
        respository.Verify(m => m.StoreAsync(It.Is(match)), times);
    }

    public void ShouldNotStoreAsync()
    {
        respository.Verify(m => m.StoreAsync(It.IsAny<Patient>()), Times.Never);
    }
}