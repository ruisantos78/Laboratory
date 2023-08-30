using Microsoft.Extensions.Caching.Memory;
using RuiSantos.Labs.Core.Repositories;

namespace RuiSantos.Labs.Core.Cache;

/// <summary>
/// Interface for accessing the domain context.
/// </summary>
public interface IRepositoryCache
{
    /// <summary>
    /// Gets all the medical specialties.
    /// </summary>
    /// <returns>A list of medical specialties.</returns>
    Task<IReadOnlySet<string>?> GetMedicalSpecialtiesAsync();

    void ClearMedicalSpecialties();
}

/// <summary>
/// Cache controller for the domain context.
/// </summary>
internal class RepositoryCache : IRepositoryCache
{
    /// <summary>
    /// The sliding expiration for the cache.
    /// </summary>
    private static readonly TimeSpan CacheSlidingExpiration = TimeSpan.FromMinutes(5);

    /// <summary>
    /// The data context.
    /// </summary>
    private readonly IMedicalSpecialityRepository specialityAdapter;

    /// <summary>
    /// The memory cache.
    /// </summary>
    private readonly IMemoryCache cache;

    /// <summary>
    /// Creates a new instance of the <see cref="RepositoryCache"/> class.
    /// </summary>
    /// <param name="specialityAdapter">The data context.</param>
    /// <param name="cache">The memory cache.</param>
    public RepositoryCache(IMedicalSpecialityRepository specialityAdapter, IMemoryCache cache)
    {
        this.specialityAdapter = specialityAdapter;
        this.cache = cache;
    }

    public void ClearMedicalSpecialties()
    {
        cache.Remove("MedicalSpecialty");
    }

    /// <summary>
    /// Gets the list of medical specialties.
    /// </summary>
    /// <returns>The list of medical specialties.</returns>
    public Task<IReadOnlySet<string>?> GetMedicalSpecialtiesAsync()
    {
        return cache.GetOrCreateAsync("MedicalSpecialty", async (entry) => 
        {
            var values = await specialityAdapter.GetAsync();
            entry.SetSlidingExpiration(CacheSlidingExpiration).SetValue(values);
            return values;
        });
    }
}
