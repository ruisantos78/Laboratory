using RuiSantos.Labs.Core.Repositories;
using RuiSantos.Labs.Data.Dynamodb.Adapters;

namespace RuiSantos.Labs.Data.Dynamodb.Repositories;

public class MedicalSpecialityRepository : IMedicalSpecialityRepository
{
    private readonly IMedicalSpecialtyAdapter _medicalSpecialtyAdapter;

    internal MedicalSpecialityRepository(IMedicalSpecialtyAdapter medicalSpecialtyAdapter)
    {
        _medicalSpecialtyAdapter = medicalSpecialtyAdapter;
    }

    public Task AddAsync(IEnumerable<string> specialties)
    {
        return _medicalSpecialtyAdapter.StoreAsync(specialties);
    }
    
    public Task RemoveAsync(string speciality)
    {
        return _medicalSpecialtyAdapter.RemoveAsync(speciality);
    }

    public Task<IEnumerable<string>> GetAsync()
    {
        return _medicalSpecialtyAdapter.LoadAsync();
    }
}