using Microsoft.Extensions.Caching.Memory;
using RuiSantos.ZocDoc.Core.Models;

namespace RuiSantos.ZocDoc.Core.Data;

public class DomainContext : IDomainContext
{
    private static readonly TimeSpan CacheSlidingExpiration = TimeSpan.FromMinutes(5);

    private readonly IDataContext context;
    private readonly IMemoryCache cache;

    public DomainContext(IDataContext context, IMemoryCache cache)
    {
        this.context = context;
        this.cache = cache;
    }

    public Task<List<MedicalSpeciality>?> GetMedicalSpecialtiesAsync()
    {
        return cache.GetOrCreateAsync(nameof(MedicalSpeciality), entry =>
        {
            var values = context.ListAsync<MedicalSpeciality>();
            entry.SetSlidingExpiration(CacheSlidingExpiration).SetValue(values);
            return values;
        });
    }
}
