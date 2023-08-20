using RuiSantos.ZocDoc.Core.Services;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.GraphQL.Schemas;

namespace RuiSantos.ZocDoc.GraphQL;

public interface IMedicalSpecialtySchemaAdapter
{
    Task<IQueryable<MedicalSpecialtySchema>> GetMedicalSpecialtiesAsync();

    Task<MedicalSpecialtySchema> CreateMedicalSpecialtyAsync(string description);

    Task<IEnumerable<MedicalSpecialtySchema>> CreateMedicalSpecialtiesAsync(IEnumerable<string> descriptions);
}

internal class MedicalSpecialtySchemaAdapter : IMedicalSpecialtySchemaAdapter
{
    private readonly IMedicalSpecialtiesService management;

    public MedicalSpecialtySchemaAdapter(IMedicalSpecialtiesService management)
    {
        this.management = management;
    }

    public async Task<MedicalSpecialtySchema> CreateMedicalSpecialtyAsync(string description)
    {
        await management.CreateMedicalSpecialtiesAsync(new() { description }); 
        return GetSchema(description);    
    }

    public async Task<IEnumerable<MedicalSpecialtySchema>>  CreateMedicalSpecialtiesAsync(IEnumerable<string> descriptions)
    {
        await management.CreateMedicalSpecialtiesAsync(descriptions.ToList()); 
        return GetSchemas(descriptions);                  
    }
    
    public async Task<IQueryable<MedicalSpecialtySchema>> GetMedicalSpecialtiesAsync()
    {
        var medicalSpecialties = await management.GetMedicalSpecialitiesAsync();
        return GetSchemas(medicalSpecialties).AsQueryable();
    }

    private MedicalSpecialtySchema GetSchema(string description) 
    {    
        return new MedicalSpecialtySchema() { Description = description };
    }

    private IEnumerable<MedicalSpecialtySchema> GetSchemas(IEnumerable<string> descriptions) 
    {    
        return descriptions.Select(GetSchema);
    }
    
    private IEnumerable<MedicalSpecialtySchema> GetSchemas(IEnumerable<MedicalSpecialty> medicalSpecialties) 
    {    
        return medicalSpecialties.Select(x => new MedicalSpecialtySchema() { Description = x.Description });
    }
}
