
namespace CachingProxy.Services;

public interface IProxyHandler
{
    Task<IResult> HandleAsync(HttpContext context, string origin);
}