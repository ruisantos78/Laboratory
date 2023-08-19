using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using RuiSantos.ZocDoc.Core.Adapters;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Data.Dynamodb.Entities;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Adapters;

public class MedicalSpecialityAdapter : IMedicalSpecialityAdapter
{
    private readonly IDynamoDBContext context;

    public MedicalSpecialityAdapter(AmazonDynamoDBClient client)
    {
        this.context = new DynamoDBContext(client);
    }

    public async Task AddAsync(MedicalSpecialty speciality)
    {
        await DomainListsDto.SetSpecialtyAsync(context, speciality.Description);
    }

    public async Task<bool> ContainsAsync(string speciality)
    {
        var result = await DomainListsDto.GetSpecialtiesAsync(context);
        return result.Values.Contains(speciality);
    }

    public async Task RemoveAsync(string speciality)
    {
        await DomainListsDto.RemoveSpecialtyAsync(context, speciality);
    }

    public async Task<List<MedicalSpecialty>> ToListAsync()
    {
        var result = await DomainListsDto.GetSpecialtiesAsync(context);
        return result.Select(s => new MedicalSpecialty
        {
            Id = s.Key,
            Description = s.Value
        }).ToList();
    }
}