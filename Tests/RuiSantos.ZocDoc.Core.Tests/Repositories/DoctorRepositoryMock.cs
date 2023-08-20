using System.Linq.Expressions;
using RuiSantos.ZocDoc.Core.Repositories;

namespace RuiSantos.ZocDoc.Core.Tests.Repositories;

public class DoctorRepositoryMock {
    private readonly Mock<IDoctorRepository> repository;

    public IDoctorRepository Object => repository.Object;
    
    public DoctorRepositoryMock()
    {
        this.repository = new Mock<IDoctorRepository>();
    }

    public void SetStoreAsyncCallback(Action<Doctor> callback)
    {
        repository.Setup(m => m.StoreAsync(It.IsAny<Doctor>()))
            .Callback<Doctor>(callback);
    }
    public void SetFindAsyncReturns(Func<string, Doctor> returns)
    {
        repository.Setup(m => m.FindAsync(It.IsAny<string>()))
            .ReturnsAsync(returns);
    }

    public void SetFindBySpecialtyAsyncReturns(List<Doctor> returns)
    {
        repository.Setup(m => m.FindBySpecialityAsync(It.IsAny<string>()))
            .ReturnsAsync(returns);
    }

    public void SetFindBySpecialtyAsyncReturns(Func<string, List<Doctor>> returns)
    {
        repository.Setup(m => m.FindBySpecialityAsync(It.IsAny<string>()))
            .ReturnsAsync(returns);
    }
    
    public void SetFindBySpecialtyWithAvailabilityAsyncReturns(List<Doctor> returns)
    {
        repository.Setup(m => m.FindBySpecialtyWithAvailabilityAsync(It.IsAny<string>(), It.IsAny<DateOnly>()))
            .ReturnsAsync(returns);
    }

    public void SetFindBySpecialtyWithAvailabilityAsyncReturns(Func<string, DateOnly, List<Doctor>> returns)
    {
        repository.Setup(m => m.FindBySpecialtyWithAvailabilityAsync(It.IsAny<string>(), It.IsAny<DateOnly>()))
            .ReturnsAsync(returns);
    }

    public void SetFindAllWithAppointmentsAsyncReturns(List<Doctor> returns)
    {
        repository.Setup(m => m.FindAllWithAppointmentsAsync(It.IsAny<IEnumerable<Appointment>>()))
            .ReturnsAsync(returns);
    }

    public void SetFindAllWithAppointmentsAsyncReturns(Func<IEnumerable<Appointment>, List<Doctor>> returns)
    {
        repository.Setup(m => m.FindAllWithAppointmentsAsync(It.IsAny<IEnumerable<Appointment>>()))
            .ReturnsAsync(returns);
    }

    public void ShouldStoreAsync()
    {
        repository.Verify(m => m.StoreAsync(It.IsAny<Doctor>()), Times.Once);
    }

    public void ShouldStoreAsync(Expression<Func<Doctor, bool>> match)
    {
        repository.Verify(m => m.StoreAsync(It.Is(match)),
            Times.Once);
    }

    public void ShouldStoreAsync(Expression<Func<Doctor, bool>> match, Func<Times> times)
    {
        repository.Verify(m => m.StoreAsync(It.Is(match)),
            times);
    }
}