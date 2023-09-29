using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using RuiSantos.Labs.Core.Repositories;
using RuiSantos.Labs.Data.Dynamodb.Adapters;
using RuiSantos.Labs.Data.Dynamodb.Entities;

namespace RuiSantos.Labs.Data.Dynamodb.Repositories;

public class MedicalSpecialityRepository : IMedicalSpecialityRepository
{
    private readonly MedicalSpecialtyAdapter medicalSpecialtyAdapter;

    public MedicalSpecialityRepository(IAmazonDynamoDB client)
    {
        this.medicalSpecialtyAdapter = new MedicalSpecialtyAdapter(client);
    }

    public async Task AddAsync(IEnumerable<string> specialties)
    {
        await medicalSpecialtyAdapter.StoreAsync(specialties);
    }
    
    public async Task RemoveAsync(string speciality)
    {
        await medicalSpecialtyAdapter.RemoveAsync(speciality);
    }

    public async Task<IReadOnlySet<string>> GetAsync()
    {
        return await medicalSpecialtyAdapter.LoadAsync();
    }
}