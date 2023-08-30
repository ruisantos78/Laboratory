using RuiSantos.ZocDoc.Core.Services;
using RuiSantos.ZocDoc.GraphQL.Schemas;

namespace RuiSantos.ZocDoc.GraphQL;

public interface IMedicalSpecialtySchemaAdapter
{
    Task<IQueryable<MedicalSpecialtySchema>> GetMedicalSpecialtiesAsync();
    Task<IEnumerable<MedicalSpecialtySchema>> CreateMedicalSpecialtiesAsync(IEnumerable<string> descriptions);
}

internal class MedicalSpecialtySchemaAdapter : IMedicalSpecialtySchemaAdapter
{
    private readonly IMedicalSpecialtiesService management;

    public MedicalSpecialtySchemaAdapter(IMedicalSpecialtiesService management)
    {
        this.management = management;
    }

    public async Task<IEnumerable<MedicalSpecialtySchema>> CreateMedicalSpecialtiesAsync(IEnumerable<string> descriptions)
    {
        await management.CreateMedicalSpecialtiesAsync(descriptions.ToList()); 
        return descriptions.Select(GetSchema);
    }

    public async Task<IQueryable<MedicalSpecialtySchema>> GetMedicalSpecialtiesAsync()
    {
        var result = await management.GetMedicalSpecialitiesAsync();
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