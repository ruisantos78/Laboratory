using RuiSantos.ZocDoc.Core.Models;

namespace RuiSantos.ZocDoc.Core.Repositories;

public interface IMedicalSpecialityRepository 
{
    Task<IReadOnlySet<string>> GetAsync();
    Task RemoveAsync(string speciality);
    Task AddAsync(IEnumerable<string> specialties);
}