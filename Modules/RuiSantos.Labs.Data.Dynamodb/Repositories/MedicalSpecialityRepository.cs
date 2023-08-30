using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using RuiSantos.Labs.Core.Repositories;
using RuiSantos.Labs.Data.Dynamodb.Entities;

namespace RuiSantos.Labs.Data.Dynamodb.Repositories;

public class MedicalSpecialityRepository : IMedicalSpecialityRepository
{
    private readonly IDynamoDBContext context;

    public MedicalSpecialityRepository(IAmazonDynamoDB client)
    {
        this.context = new DynamoDBContext(client);
    }

    public async Task AddAsync(IEnumerable<string> specialties)
    {
        await DictionaryDto.SetSpecialtyAsync(context, specialties);
    }

    public async Task RemoveAsync(string speciality)
    {
        await DictionaryDto.RemoveSpecialtyAsync(context, speciality);
    }

    public async Task<IReadOnlySet<string>> GetAsync()
    {
        return await DictionaryDto.GetSpecialtiesAsync(context);
    }
}