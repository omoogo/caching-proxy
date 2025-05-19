using CachingProxy.Data;
using CachingProxy.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CachingProxy.UnitTests.Data;
public class CacheRepositoryTests
{
    [Fact]
    public async Task ClearCacheAsync_RemovesAllEntries()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CacheContext>()
            .UseInMemoryDatabase(databaseName: "ClearCacheAsync_RemovesAllEntries")
            .Options;
        using (var context = new CacheContext(options))
        {
            context.CachedResponses.Add(new CachedResponse { CacheKey = "key1", StatusCode = 200, ContentType = "text/html", Body = "body1", CreatedAt = DateTime.UtcNow });
            context.CachedResponses.Add(new CachedResponse { CacheKey = "key2", StatusCode = 200, ContentType = "text/html", Body = "body2", CreatedAt = DateTime.UtcNow });
            context.SaveChanges();
        }

        // Act
        using (var context = new CacheContext(options))
        {
            var repository = new CacheRepository(context);
            await repository.ClearCacheAsync();
        }

        // Assert
        using (var context = new CacheContext(options))
        {
            var cachedResponses = await context.CachedResponses.ToListAsync();
            Assert.Empty(cachedResponses);
        }
    }

    [Fact]
    public async Task AddCachedResponse_SavesTheEntry()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CacheContext>()
            .UseInMemoryDatabase(databaseName: "AddCachedResponse_AddsTheResponseToTheContext")
            .Options;

        CachedResponse expectedCachedResponse = new()
        {
            CacheKey = "key1",
            StatusCode = 200,
            ContentType = "text/html",
            Body = "body1",
            CreatedAt = DateTime.UtcNow
        };

        // Act
        using (var context = new CacheContext(options))
        {
            var repository = new CacheRepository(context);
            await repository.AddCachedResponseAsync(expectedCachedResponse);
        }

        // Assert
        using (var context = new CacheContext(options))
        {
            var cachedResponseFromDb = await context.CachedResponses.FirstOrDefaultAsync();
            Assert.NotNull(cachedResponseFromDb);
            Assert.Equal(expectedCachedResponse.CacheKey, cachedResponseFromDb.CacheKey);
            Assert.Equal(expectedCachedResponse.StatusCode, cachedResponseFromDb.StatusCode);
            Assert.Equal(expectedCachedResponse.ContentType, cachedResponseFromDb.ContentType);
            Assert.Equal(expectedCachedResponse.Body, cachedResponseFromDb.Body);
        }
    }

    [Fact]
    public async Task GetCachedResponseAsync_ReturnsTheSavedEntry()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CacheContext>()
            .UseInMemoryDatabase(databaseName: "GetCachedResponseAsync_ReturnsTheSavedEntry")
            .Options;

        using (var context = new CacheContext(options))
        {
            context.CachedResponses.Add(new CachedResponse { CacheKey = "key1", StatusCode = 200, ContentType = "text/html", Body = "body1", CreatedAt = DateTime.UtcNow });
            context.CachedResponses.Add(new CachedResponse { CacheKey = "key2", StatusCode = 200, ContentType = "text/html", Body = "body2", CreatedAt = DateTime.UtcNow });
            await context.SaveChangesAsync();
        }

        // Act
        CachedResponse? cachedReponseFromDb;
        using (var context = new CacheContext(options))
        {
            var repository = new CacheRepository(context);
            cachedReponseFromDb = await repository.GetCachedResponseAsync("key2");
        }

        // Assert
        Assert.NotNull(cachedReponseFromDb);
        Assert.Equal("key2", cachedReponseFromDb.CacheKey);
        Assert.Equal(200, cachedReponseFromDb.StatusCode);
        Assert.Equal("text/html", cachedReponseFromDb.ContentType);
        Assert.Equal("body2", cachedReponseFromDb.Body);
    }

}
