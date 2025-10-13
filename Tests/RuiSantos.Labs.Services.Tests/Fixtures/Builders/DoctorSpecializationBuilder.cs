using Amazon.DynamoDBv2.DataModel;
using RuiSantos.Labs.Repository.DynamoDb.Entities;

namespace RuiSantos.Labs.Services.Tests.Fixtures.Builders;

internal class DoctorSpecializationBuilder
{
    private DoctorEntity? _doctorEntity;
    private string[] _specialties = ["Cardiologist", "Dermatologist", "Gastroenterologist"];
    
    public DoctorSpecializationBuilder WithDoctor(DoctorEntity doctorEntity)
    {
        _doctorEntity = doctorEntity;
        return this;
    }
    
    public DoctorSpecializationBuilder WithSpecialties(string[] specialties)
    {
        _specialties = specialties;
        return this;
    }
    
    public async Task<DoctorSpecializationEntity[]> BuildAsync(DynamoDBContext context)
    {
        _doctorEntity ??= await new DoctorBuilder().BuildAsync(context);

        var entities = _specialties.Select(specialty => new DoctorSpecializationEntity
        {
            DoctorId = _doctorEntity.Id, 
            Specialization = specialty
        }).ToArray();
        
        var writer = context.CreateBatchWrite<DoctorSpecializationEntity>();
        writer.AddPutItems(entities);
        await writer.ExecuteAsync();

        return entities;
    }
}