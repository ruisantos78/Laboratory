using RuiSantos.Labs.Core.Services;
using RuiSantos.Labs.GraphQL.Schemas;

namespace RuiSantos.Labs.GraphQL;

public interface IMedicalSpecialtySchemaAdapter
{
    Task<IQueryable<MedicalSpecialtySchema>> GetMedicalSpecialtiesAsync();
    Task<IEnumerable<MedicalSpecialtySchema>> CreateMedicalSpecialtiesAsync(IEnumerable<string> descriptions);
    Task<IEnumerable<MedicalSpecialtySchema>> RemoveMedicalSpecialtiesAsync(string description);
}

internal class MedicalSpecialtySchemaAdapter : IMedicalSpecialtySchemaAdapter
{
    private readonly IMedicalSpecialtiesService service;

    public MedicalSpecialtySchemaAdapter(IMedicalSpecialtiesService service)
    {
        this.service = service;
    }

    public async Task<IEnumerable<MedicalSpecialtySchema>> CreateMedicalSpecialtiesAsync(IEnumerable<string> descriptions)
    {
        await service.CreateMedicalSpecialtiesAsync(descriptions.ToList()); 
        return descriptions.Select(GetSchema);
    }

    public async Task<IEnumerable<MedicalSpecialtySchema>> RemoveMedicalSpecialtiesAsync(string description)
    {
        await service.RemoveMedicalSpecialtiesAsync(description); 
        return new List<MedicalSpecialtySchema> {GetSchema(description)};
    }

    public async Task<IQueryable<MedicalSpecialtySchema>> GetMedicalSpecialtiesAsync()
    {
        var result = await service.GetMedicalSpecialitiesAsync();
        return result.Select(GetSchema).AsQueryable();
    }

    private MedicalSpecialtySchema GetSchema(string description)
    {
        return new MedicalSpecialtySchema
        {
            Description = description
        };
    }
}