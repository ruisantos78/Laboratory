using System.Linq.Expressions;
using RuiSantos.ZocDoc.Core.Adapters;

namespace RuiSantos.ZocDoc.Core.Tests.Adapters;

public class PatientAdapterMock
{
    private readonly Mock<IPatientAdapter> adapter;

    public IPatientAdapter Object => adapter.Object;

    public PatientAdapterMock()
    {
        this.adapter = new Mock<IPatientAdapter>();
    }
    
    public void SetFindAsyncReturns(Func<string, Patient?> returns)
    {
        adapter.Setup(m => m.FindAsync(It.IsAny<string>()))
            .ReturnsAsync(returns);
    }

    public void SetFindAllWithAppointmentsAsyncReturns(List<Patient> returns)
    {
        adapter.Setup(m => m.FindAllWithAppointmentsAsync(It.IsAny<IEnumerable<Appointment>>()))
            .ReturnsAsync(returns);
    }

    public void SetFindAllWithAppointmentsAsyncReturns(Func<IEnumerable<Appointment>, List<Patient>> returns)
    {
        adapter.Setup(m => m.FindAllWithAppointmentsAsync(It.IsAny<IEnumerable<Appointment>>()))
            .ReturnsAsync(returns);
    }

    public void SetStoreAsyncCallback(Action<Patient> callback)
    {
        adapter.Setup(m => m.StoreAsync(It.IsAny<Patient>()))
            .Callback<Patient>(callback);
    }

    public void ShouldStoreAsync(Expression<Func<Patient, bool>> match)
    {
        adapter.Verify(m => m.StoreAsync(It.Is(match)), Times.Once);
    }

    public void ShouldStoreAsync(Expression<Func<Patient, bool>> match, Func<Times> times)
    {
        adapter.Verify(m => m.StoreAsync(It.Is(match)), times);
    }

    public void ShouldNotStoreAsync()
    {
        adapter.Verify(m => m.StoreAsync(It.IsAny<Patient>()), Times.Never);
    }
}