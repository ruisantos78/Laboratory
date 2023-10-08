using NSubstitute;
using RuiSantos.Labs.Core.Repositories;
using RuiSantos.Labs.Data.Dynamodb.Adapters;
using RuiSantos.Labs.Data.Dynamodb.Repositories;

namespace RuiSantos.Labs.Tests.Asserts.Repositories;

internal class MedicalSpecialityRepositoryAsserts
{
    private readonly IMedicalSpecialtyAdapter _medicalSpecialtyAdapter = Substitute.For<IMedicalSpecialtyAdapter>();

    public IMedicalSpecialityRepository GetRepository() => new MedicalSpecialityRepository(
        _medicalSpecialtyAdapter
    );

    public Task ShouldAddAsync(IEnumerable<string> specialties, bool received = true)
    {
        return received
            ? _medicalSpecialtyAdapter.Received().StoreAsync(specialties)
            : _medicalSpecialtyAdapter.DidNotReceive().StoreAsync(specialties);
    }

    public Task ShouldRemoveAsync(string specialty, bool received = true)
    {
        return received
            ? _medicalSpecialtyAdapter.Received().RemoveAsync(specialty)
            : _medicalSpecialtyAdapter.DidNotReceive().RemoveAsync(specialty);
    }

    public void WhenAddAsyncThrows(IEnumerable<string> specialties, Exception ex)
    {
        _medicalSpecialtyAdapter.When(x => x.StoreAsync(specialties))
            .Throw(ex);
    }

    public void WhenRemoveAsyncThrows(string specialty, Exception ex)
    {
        _medicalSpecialtyAdapter.When(x => x.RemoveAsync(specialty))
            .Throw(ex);
    }
}

