using RuiSantos.Labs.Core.Services;
using RuiSantos.Labs.GraphQL.Schemas;

namespace RuiSantos.Labs.GraphQL.Adapters;

[Adapter(typeof(MedicalSpecialtySchemaAdapter))]
public interface IMedicalSpecialtySchemaAdapter
{
    Task<IEnumerable<MedicalSpecialtySchema>> FindAllAsync();
    Task<IEnumerable<MedicalSpecialtySchema>> StoreAsync(IEnumerable<string> descriptions);
    Task<IEnumerable<MedicalSpecialtySchema>> RemoveAsync(string description);
}

internal class MedicalSpecialtySchemaAdapter : AdapterModelSchema<string, MedicalSpecialtySchema>, IMedicalSpecialtySchemaAdapter
{
    private readonly IMedicalSpecialtiesService _service;

    public MedicalSpecialtySchemaAdapter(IMedicalSpecialtiesService service)
    {
        _service = service;
    }

    protected override MedicalSpecialtySchema GetSchema(string description) => new()
    {
        Description = description
    };

    protected override string GetModel(MedicalSpecialtySchema schema) => schema.Description;
    
    public async Task<IEnumerable<MedicalSpecialtySchema>> StoreAsync(IEnumerable<string> descriptions)
    {
        var descriptionsList = descriptions.ToList();

        await _service.CreateMedicalSpecialtiesAsync(descriptionsList);
        return descriptionsList.Select(GetSchema);
    }

    public async Task<IEnumerable<MedicalSpecialtySchema>> RemoveAsync(string description)
    {
        await _service.RemoveMedicalSpecialtiesAsync(description);
        return new[] { GetSchema(description) };
    }

    public async Task<IEnumerable<MedicalSpecialtySchema>> FindAllAsync()
    {
        var result = await _service.GetMedicalSpecialitiesAsync();
        return result.Select(GetSchema).OrderBy(x => x.Description);
    }
}