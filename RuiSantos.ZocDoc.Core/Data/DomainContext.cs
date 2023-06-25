using Microsoft.Extensions.Caching.Memory;
using RuiSantos.ZocDoc.Core.Adapters;
using RuiSantos.ZocDoc.Core.Models;

namespace RuiSantos.ZocDoc.Core.Data;

/// <summary>
/// Interface for accessing the domain context.
/// </summary>
public interface IDomainContext
{
    /// <summary>
    /// Gets all the medical specialties.
    /// </summary>
    /// <returns>A list of medical specialties.</returns>
    Task<List<MedicalSpeciality>?> GetMedicalSpecialtiesAsync();
}

/// <summary>
/// Cache controller for the domain context.
/// </summary>
internal class DomainContext : IDomainContext
{
    /// <summary>
    /// The sliding expiration for the cache.
    /// </summary>
    private static readonly TimeSpan CacheSlidingExpiration = TimeSpan.FromMinutes(5);

    /// <summary>
    /// The data context.
    /// </summary>
    private readonly IMedicalSpecialityAdapter specialityAdapter;

    /// <summary>
    /// The memory cache.
    /// </summary>
    private readonly IMemoryCache cache;

    /// <summary>
    /// Creates a new instance of the <see cref="DomainContext"/> class.
    /// </summary>
    /// <param name="specialityAdapter">The data context.</param>
    /// <param name="cache">The memory cache.</param>
    public DomainContext(IMedicalSpecialityAdapter specialityAdapter, IMemoryCache cache)
    {
        this.specialityAdapter = specialityAdapter;
        this.cache = cache;
    }

    /// <summary>
    /// Gets the list of medical specialties.
    /// </summary>
    /// <returns>The list of medical specialties.</returns>
    public Task<List<MedicalSpeciality>?> GetMedicalSpecialtiesAsync()
    {
        return cache.GetOrCreateAsync(nameof(MedicalSpeciality), async (entry) => 
        {
            var values = await specialityAdapter.ToListAsync();
            entry.SetSlidingExpiration(CacheSlidingExpiration).SetValue(values);
            return values;
        });
    }
}
