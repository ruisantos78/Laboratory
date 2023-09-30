using Amazon.DynamoDBv2;
using RuiSantos.Labs.Core.Repositories;
using RuiSantos.Labs.Data.Dynamodb.Adapters;

namespace RuiSantos.Labs.Data.Dynamodb.Repositories;

public class MedicalSpecialityRepository : IMedicalSpecialityRepository
{
    private readonly MedicalSpecialtyAdapter _medicalSpecialtyAdapter;

    public MedicalSpecialityRepository(IAmazonDynamoDB client)
    {
        _medicalSpecialtyAdapter = new MedicalSpecialtyAdapter(client);
    }

    public Task AddAsync(IEnumerable<string> specialties)
    {
        return _medicalSpecialtyAdapter.StoreAsync(specialties);
    }
    
    public Task RemoveAsync(string speciality)
    {
        return _medicalSpecialtyAdapter.RemoveAsync(speciality);
    }

    public Task<IReadOnlySet<string>> GetAsync()
    {
        return _medicalSpecialtyAdapter.LoadAsync();
    }
}