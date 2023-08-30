using RuiSantos.Labs.Core.Models;

namespace RuiSantos.Labs.Core.Repositories;

public interface IMedicalSpecialityRepository 
{
    Task<IReadOnlySet<string>> GetAsync();
    Task RemoveAsync(string speciality);
    Task AddAsync(IEnumerable<string> specialties);
}