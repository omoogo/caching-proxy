using CachingProxy.Data.Entities;

namespace CachingProxy.Data;
public interface ICacheRepository
{
    Task AddCachedResponseAsync(CachedResponse cachedResponse);
    Task ClearCacheAsync();
    Task<CachedResponse?> GetCachedResponseAsync(string cacheKey);
}