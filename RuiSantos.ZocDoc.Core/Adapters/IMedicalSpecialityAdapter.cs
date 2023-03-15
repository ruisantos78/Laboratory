using RuiSantos.ZocDoc.Core.Models;

namespace RuiSantos.ZocDoc.Core.Adapters;

public interface IMedicalSpecialityAdapter 
{
    Task<List<MedicalSpeciality>> ToListAsync();

    Task<bool> ContainsAsync(string speciality);

    Task AddAsync(MedicalSpeciality speciality);

    Task RemoveAsync(string speciality);
}