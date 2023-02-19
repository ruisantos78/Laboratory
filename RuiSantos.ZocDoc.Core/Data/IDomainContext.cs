using RuiSantos.ZocDoc.Core.Models;

namespace RuiSantos.ZocDoc.Core.Data;

public interface IDomainContext
{
    Task<List<MedicalSpeciality>?> GetMedicalSpecialtiesAsync();
}