using CachingProxy.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CachingProxy.Data;

public class CacheContext : DbContext
{
    public CacheContext(DbContextOptions<CacheContext> options) : base(options)
    {
    }

    public DbSet<CachedResponse> CachedResponses { get; set; }
}
