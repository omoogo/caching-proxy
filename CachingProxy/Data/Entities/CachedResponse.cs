using System.ComponentModel.DataAnnotations;

namespace CachingProxy.Data.Entities;

public class CachedResponse
{
    public int Id { get; set; }
    public required string CacheKey { get; set; }
    public int StatusCode { get; set; }
    public string? ContentType { get; set; }
    public string? Body { get; set; }
    public DateTime CreatedAt { get; set; }
}
