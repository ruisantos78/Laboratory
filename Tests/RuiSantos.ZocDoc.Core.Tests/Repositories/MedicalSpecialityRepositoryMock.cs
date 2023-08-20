using RuiSantos.ZocDoc.Core.Repositories;

namespace RuiSantos.ZocDoc.Core.Tests.Repositories;

public class MedicalSpecialityRepositoryMock
{
	private readonly Mock<IMedicalSpecialityRepository> repository;

	public IMedicalSpecialityRepository Object => repository.Object;

    public MedicalSpecialityRepositoryMock()
	{
		this.repository = new Mock<IMedicalSpecialityRepository>();
	}

	public void SetAddAsyncCallback(Action<MedicalSpecialty> callback)
	{
		repository.Setup(m => m.AddAsync(It.IsAny<MedicalSpecialty>()))
			.Callback<MedicalSpecialty>(callback);
	}

	public void SetRemoveAsyncCallback(Action<string> callback)
	{
        repository.Setup(m => m.RemoveAsync(It.IsAny<string>()))
			.Callback<string>(callback);
    }

	public void SetContainsAsyncReturns(Func<string, bool> returns)
	{
        repository.Setup(m => m.ContainsAsync(It.IsAny<string>()))
           .ReturnsAsync(returns);
    }

	public void SetToListAsyncReturns(List<MedicalSpecialty> returns)
	{
        repository.Setup(m => m.ToListAsync())
			.ReturnsAsync(returns);
    }

	public void ShouldAddAsync(int count)
	{
		repository.Verify(m => m.AddAsync(It.IsAny<MedicalSpecialty>()),
			Times.Exactly(count));
    }

	public void ShouldNotAddAsync()
	{
        repository.Verify(m => m.AddAsync(It.IsAny<MedicalSpecialty>()),
			Times.Never);
    }

	public void ShouldRemoveAsync()
	{
        repository.Verify(m => m.RemoveAsync(It.IsAny<string>()), Times.Once);
    }

    public void ShouldRemoveAsync(string specialty)
    {
        repository.Verify(m => m.RemoveAsync(specialty),
			Times.Once);
    }

    public void ShouldNotRemoveAsync()
    {
        repository.Verify(m => m.RemoveAsync(It.IsAny<string>()), Times.Never);
    }
}