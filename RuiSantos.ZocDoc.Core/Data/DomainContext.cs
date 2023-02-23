using Microsoft.Extensions.Caching.Memory;
using RuiSantos.ZocDoc.Core.Models;

namespace RuiSantos.ZocDoc.Core.Data;

/// <summary>
/// Cache controller for the domain context.
/// </summary>
public class DomainContext : IDomainContext
{
    /// <summary>
    /// The sliding expiration for the cache.
    /// </summary>
    private static readonly TimeSpan CacheSlidingExpiration = TimeSpan.FromMinutes(5);

    /// <summary>
    /// The data context.
    /// </summary>
    private readonly IDataContext context;

    /// <summary>
    /// The memory cache.
    /// </summary>
    private readonly IMemoryCache cache;

    /// <summary>
    /// Creates a new instance of the <see cref="DomainContext"/> class.
    /// </summary>
    /// <param name="context">The data context.</param>
    /// <param name="cache">The memory cache.</param>
    public DomainContext(IDataContext context, IMemoryCache cache)
    {
        this.context = context;
        this.cache = cache;
    }

    /// <summary>
    /// Gets the list of medical specialties.
    /// </summary>
    /// <returns>The list of medical specialties.</returns>
    public Task<List<MedicalSpeciality>?> GetMedicalSpecialtiesAsync()
    {
        return cache.GetOrCreateAsync(nameof(MedicalSpeciality), entry =>
        {
            var values = context.ToListAsync<MedicalSpeciality>();
            entry.SetSlidingExpiration(CacheSlidingExpiration).SetValue(values);
            return values;
        });
    }
}
