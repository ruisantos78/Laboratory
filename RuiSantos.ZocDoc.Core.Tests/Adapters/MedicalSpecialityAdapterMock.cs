using RuiSantos.ZocDoc.Core.Adapters;

namespace RuiSantos.ZocDoc.Core.Tests.Adapters;

public class MedicalSpecialityAdapterMock
{
	private readonly Mock<IMedicalSpecialityAdapter> adapter;

	public IMedicalSpecialityAdapter Object => adapter.Object;

    public MedicalSpecialityAdapterMock()
	{
		this.adapter = new Mock<IMedicalSpecialityAdapter>();
	}

	public void SetAddAsyncCallback(Action<MedicalSpecialty> callback)
	{
		adapter.Setup(m => m.AddAsync(It.IsAny<MedicalSpecialty>()))
			.Callback<MedicalSpecialty>(callback);
	}

	public void SetRemoveAsyncCallback(Action<string> callback)
	{
        adapter.Setup(m => m.RemoveAsync(It.IsAny<string>()))
			.Callback<string>(callback);
    }

	public void SetContainsAsyncReturns(Func<string, bool> returns)
	{
        adapter.Setup(m => m.ContainsAsync(It.IsAny<string>()))
           .ReturnsAsync(returns);
    }

	public void SetToListAsyncReturns(List<MedicalSpecialty> returns)
	{
        adapter.Setup(m => m.ToListAsync())
			.ReturnsAsync(returns);
    }

	public void ShouldAddAsync(int count)
	{
		adapter.Verify(m => m.AddAsync(It.IsAny<MedicalSpecialty>()),
			Times.Exactly(count));
    }

	public void ShouldNotAddAsync()
	{
        adapter.Verify(m => m.AddAsync(It.IsAny<MedicalSpecialty>()),
			Times.Never);
    }

	public void ShouldRemoveAsync()
	{
        adapter.Verify(m => m.RemoveAsync(It.IsAny<string>()), Times.Once);
    }

    public void ShouldRemoveAsync(string specialty)
    {
        adapter.Verify(m => m.RemoveAsync(specialty),
			Times.Once);
    }

    public void ShouldNotRemoveAsync()
    {
        adapter.Verify(m => m.RemoveAsync(It.IsAny<string>()), Times.Never);
    }
}