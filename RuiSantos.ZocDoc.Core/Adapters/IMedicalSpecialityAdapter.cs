using RuiSantos.ZocDoc.Core.Models;

namespace RuiSantos.ZocDoc.Core.Adapters;

public interface IMedicalSpecialityAdapter 
{
    Task<List<MedicalSpecialty>> ToListAsync();

    Task<bool> ContainsAsync(string speciality);

    Task AddAsync(MedicalSpecialty speciality);

    Task RemoveAsync(string speciality);
}