using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using RuiSantos.ZocDoc.Core.Repositories;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Data.Dynamodb.Entities;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Repositories;

public class MedicalSpecialityRepository : IMedicalSpecialityRepository
{
    private readonly IDynamoDBContext context;

    public MedicalSpecialityRepository(IAmazonDynamoDB client)
    {
        this.context = new DynamoDBContext(client);
    }

    public async Task AddAsync(MedicalSpecialty speciality)
    {
        await DictionaryDto.SetSpecialtyAsync(context, speciality.Description);
    }

    public async Task<bool> ContainsAsync(string speciality)
    {
        var result = await DictionaryDto.GetSpecialtiesAsync(context);
        return result.Any(x => x.Description == speciality);
    }

    public async Task RemoveAsync(string speciality)
    {
        await DictionaryDto.RemoveSpecialtyAsync(context, speciality);
    }

    public async Task<List<MedicalSpecialty>> ToListAsync()
    {
        return await DictionaryDto.GetSpecialtiesAsync(context);
    }
}