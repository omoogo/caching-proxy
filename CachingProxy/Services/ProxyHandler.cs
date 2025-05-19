using CachingProxy.Data;
using CachingProxy.Data.Entities;

namespace CachingProxy.Services;

public class ProxyHandler : IProxyHandler
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ICacheRepository _cacheRepository;

    public ProxyHandler(IHttpClientFactory httpClientFactory, ICacheRepository cacheRepository)
    {
        _httpClientFactory = httpClientFactory;
        _cacheRepository = cacheRepository;
    }

    public async Task<IResult> HandleAsync(
        HttpContext context,
        string origin)
    {
        var cacheKey = GenerateCacheKey(context, origin);
        var cache = await GetCachedResponseAsync(cacheKey, context);

        if (cache is not null)
        {
            return cache;
        }

        var request = await BuildHttpRequestAsync(context, origin);
        var response = await SendRequestAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();

        await CacheResponseAsync(cacheKey, response, responseBody);

        CopyResponseHeaders(context, response);

        context.Response.Headers["X-Cache"] = "MISS";
        context.Response.StatusCode = (int)response.StatusCode;

        return Results.Content(
            responseBody,
            response.Content.Headers.ContentType?.ToString()
        );
    }

    public string GenerateCacheKey(HttpContext context, string origin)
    {
        return origin + context.Request.Path + context.Request.QueryString;
    }

    public void CopyResponseHeaders(HttpContext context, HttpResponseMessage response)
    {
        string[] excludedHeaders = [
            "Transfer-Encoding",
            "Content-Length",
            "Content-Type",
            "Server",
            "Date",
            "Connection"
        ];

        foreach (var header in response.Headers)
        {
            if (!excludedHeaders.Contains(header.Key, StringComparer.OrdinalIgnoreCase))
            {
                context.Response.Headers[header.Key] = header.Value.ToArray();
            }
        }

        foreach (var header in response.Content.Headers)
        {
            if (!excludedHeaders.Contains(header.Key, StringComparer.OrdinalIgnoreCase))
            {
                context.Response.Headers[header.Key] = header.Value.ToArray();
            }
        }
    }

    public async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage httpRequestMessage)
    {
        var httpClient = _httpClientFactory.CreateClient();
        var response = await httpClient.SendAsync(httpRequestMessage);
        return response;
    }

    public async Task<HttpRequestMessage> BuildHttpRequestAsync(HttpContext context, string origin)
    {
        var method = context.Request.Method;
        var path = context.Request.Path + context.Request.QueryString;
        var body = await new StreamReader(context.Request.Body).ReadToEndAsync() ?? "";
        var requstUrl = $"{origin}{path}";

        var httpRequestMessage = new HttpRequestMessage(
            new HttpMethod(method),
            requstUrl)
        {
            Content = new StreamContent(context.Request.Body),
        };
        return httpRequestMessage;
    }

    private async Task CacheResponseAsync(string cacheKey, HttpResponseMessage response, string responseBody)
    {
        var newCachedResponse = new CachedResponse()
        {
            CacheKey = cacheKey,
            StatusCode = (int)response.StatusCode,
            ContentType = response.Content.Headers.ContentType?.ToString(),
            Body = responseBody,
            CreatedAt = DateTime.UtcNow
        };

        await _cacheRepository.AddCachedResponseAsync(newCachedResponse);
    }

    public async Task<IResult?> GetCachedResponseAsync(string cacheKey, HttpContext context)
    {
        var cached = await _cacheRepository.GetCachedResponseAsync(cacheKey);

        if (cached is null)
        {
            return null;
        }

        context.Response.Headers["X-Cache"] = "HIT";
        context.Response.StatusCode = cached.StatusCode;

        return Results.Content(cached.Body, cached.ContentType, statusCode: cached.StatusCode);
    }

    
}
