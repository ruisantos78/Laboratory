using Amazon.DynamoDBv2.DataModel;
using RuiSantos.Labs.Repository.DynamoDb.Entities;

namespace RuiSantos.Labs.Services.Tests.Fixtures.Builders;

internal class DoctorSpecializationEntityBuilder(DoctorEntity doctorEntity)
{
    private HashSet<string> _specialties = ["Cardiologist", "Dermatologist", "Gastroenterologist"];
    
    public DoctorSpecializationEntityBuilder WithSpecializations(params string[] specialties)
    {
        _specialties = specialties.ToHashSet();
        return this;
    }
    
    public async Task<DoctorSpecializationEntity[]> BuildAsync(DynamoDBContext context)
    {
        var entities = _specialties.Select(specialty => new DoctorSpecializationEntity
        {
            DoctorId = doctorEntity.Id, 
            Specialization = specialty.ToLower(),
            Description = specialty
        }).ToArray();
        
        var writer = context.CreateBatchWrite<DoctorSpecializationEntity>();
        writer.AddPutItems(entities);
        await writer.ExecuteAsync();

        return entities;
    }
}