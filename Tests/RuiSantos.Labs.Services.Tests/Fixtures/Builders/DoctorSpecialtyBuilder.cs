using Amazon.DynamoDBv2.DataModel;
using RuiSantos.Labs.Data.Dynamodb.Entities;

namespace RuiSantos.Labs.Services.Tests.Fixtures.Builders;

internal class DoctorSpecialtyBuilder
{
    private DoctorEntity? _doctorEntity;
    private string[] _specialties = ["Cardiologist", "Dermatologist", "Gastroenterologist"];
    
    public DoctorSpecialtyBuilder WithDoctor(DoctorEntity doctorEntity)
    {
        _doctorEntity = doctorEntity;
        return this;
    }
    
    public DoctorSpecialtyBuilder WithSpecialties(string[] specialties)
    {
        _specialties = specialties;
        return this;
    }
    
    public async Task<DoctorSpecialtyEntity[]> BuildAsync(DynamoDBContext context)
    {
        _doctorEntity ??= await new DoctorBuilder().BuildAsync(context);

        var entities = _specialties.Select(specialty => new DoctorSpecialtyEntity
        {
            DoctorId = _doctorEntity.Id, 
            Specialty = specialty
        }).ToArray();
        
        var writer = context.CreateBatchWrite<DoctorSpecialtyEntity>();
        writer.AddPutItems(entities);
        await writer.ExecuteAsync();

        return entities;
    }
}