using System.Linq.Expressions;
using Moq;
using RuiSantos.ZocDoc.Core.Adapters;
using RuiSantos.ZocDoc.Core.Models;

namespace RuiSantos.ZocDoc.Core.Tests.Adapters;

public class DoctorAdapterMock {
    private readonly Mock<IDoctorAdapter> adapter;

    public IDoctorAdapter Object => adapter.Object;
    
    public DoctorAdapterMock()
    {
        this.adapter = new Mock<IDoctorAdapter>();
    }

    public void SetStoreAsyncCallback(Action<Doctor> callback)
    {
        adapter.Setup(m => m.StoreAsync(It.IsAny<Doctor>()))
            .Callback<Doctor>(callback);
    }
    public void SetFindAsyncReturns(Func<string, Doctor> returns)
    {
        adapter.Setup(m => m.FindAsync(It.IsAny<string>()))
            .ReturnsAsync(returns);
    }

    public void SetFindBySpecialtyAsyncReturns(List<Doctor> returns)
    {
        adapter.Setup(m => m.FindBySpecialityAsync(It.IsAny<string>()))
            .ReturnsAsync(returns);
    }

    public void SetFindBySpecialtyAsyncReturns(Func<string, List<Doctor>> returns)
    {
        adapter.Setup(m => m.FindBySpecialityAsync(It.IsAny<string>()))
            .ReturnsAsync(returns);
    }
    
    public void SetFindBySpecialtyWithAvailabilityAsyncReturns(List<Doctor> returns)
    {
        adapter.Setup(m => m.FindBySpecialtyWithAvailabilityAsync(It.IsAny<string>(), It.IsAny<DateOnly>()))
            .ReturnsAsync(returns);
    }

    public void SetFindBySpecialtyWithAvailabilityAsyncReturns(Func<string, DateOnly, List<Doctor>> returns)
    {
        adapter.Setup(m => m.FindBySpecialtyWithAvailabilityAsync(It.IsAny<string>(), It.IsAny<DateOnly>()))
            .ReturnsAsync(returns);
    }

    public void SetFindAllWithAppointmentsAsyncReturns(List<Doctor> returns)
    {
        adapter.Setup(m => m.FindAllWithAppointmentsAsync(It.IsAny<IEnumerable<Appointment>>()))
            .ReturnsAsync(returns);
    }

    public void SetFindAllWithAppointmentsAsyncReturns(Func<IEnumerable<Appointment>, List<Doctor>> returns)
    {
        adapter.Setup(m => m.FindAllWithAppointmentsAsync(It.IsAny<IEnumerable<Appointment>>()))
            .ReturnsAsync(returns);
    }

    public void ShouldStoreAsync()
    {
        adapter.Verify(m => m.StoreAsync(It.IsAny<Doctor>()), Times.Once);
    }

    public void ShouldStoreAsync(Expression<Func<Doctor, bool>> match)
    {
        adapter.Verify(m => m.StoreAsync(It.Is(match)),
            Times.Once);
    }

    public void ShouldStoreAsync(Expression<Func<Doctor, bool>> match, Func<Times> times)
    {
        adapter.Verify(m => m.StoreAsync(It.Is(match)),
            times);
    }
}