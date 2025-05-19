using CachingProxy.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CachingProxy.Data;

public class CacheRepository : ICacheRepository
{
    private readonly CacheContext _context;
    private static bool _dbInitialized;

    public CacheRepository(CacheContext context)
    {
        _context = context;

        if (!_dbInitialized)
        {
            _context.Database.EnsureCreated();
            _dbInitialized = true;
        }
    }

    public async Task ClearCacheAsync()
    {
        var cachedResponses = await _context.CachedResponses.ToListAsync();
        _context.CachedResponses.RemoveRange(cachedResponses);
        await _context.SaveChangesAsync();
    }

    public async Task AddCachedResponseAsync(CachedResponse cachedResponse)
    {
        _context.CachedResponses.Add(cachedResponse);
        await _context.SaveChangesAsync();
    }

    public async Task<CachedResponse?> GetCachedResponseAsync(string cacheKey)
    {
        return await _context.CachedResponses
            .FirstOrDefaultAsync(x => x.CacheKey == cacheKey);
    }
}
