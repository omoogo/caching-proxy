using CachingProxy.Services;

namespace CachingProxy.Endpoints;

public static class ProxyEndpoints
{
    public static void AddProxyEndpoints(this WebApplication app, string origin)
    {
        app.Map("{**path}", async (HttpContext context, IProxyHandler proxyHandler) =>
        {
            return await proxyHandler.HandleAsync(context, origin);
        });
    }
}
