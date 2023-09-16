using RuiSantos.Labs.Core.Services;
using RuiSantos.Labs.GraphQL.Schemas;

namespace RuiSantos.Labs.GraphQL;

[Adapter(typeof(MedicalSpecialtySchemaAdapter))]
public interface IMedicalSpecialtySchemaAdapter
{
    Task<IEnumerable<MedicalSpecialtySchema>> FindAllAsync();
    Task<IEnumerable<MedicalSpecialtySchema>> StoreAsync(IEnumerable<string> descriptions);
    Task<IEnumerable<MedicalSpecialtySchema>> RemoveAsync(string description);
}

internal class MedicalSpecialtySchemaAdapter : IMedicalSpecialtySchemaAdapter
{
    private readonly IMedicalSpecialtiesService service;

    public MedicalSpecialtySchemaAdapter(IMedicalSpecialtiesService service)
    {
        this.service = service;
    }

    public async Task<IEnumerable<MedicalSpecialtySchema>> StoreAsync(IEnumerable<string> descriptions)
    {
        await service.CreateMedicalSpecialtiesAsync(descriptions.ToList()); 
        return descriptions.Select(GetSchema);
    }

    public async Task<IEnumerable<MedicalSpecialtySchema>> RemoveAsync(string description)
    {
        await service.RemoveMedicalSpecialtiesAsync(description); 
        return new List<MedicalSpecialtySchema> {GetSchema(description)};
    }

    public async Task<IEnumerable<MedicalSpecialtySchema>> FindAllAsync()
    {
        var result = await service.GetMedicalSpecialitiesAsync();
        return result.Select(GetSchema).OrderBy(x => x.Description);
    }

    private MedicalSpecialtySchema GetSchema(string description)
    {
        return new MedicalSpecialtySchema
        {
            Description = description
        };
    }
}