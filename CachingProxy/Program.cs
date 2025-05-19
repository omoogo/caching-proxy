using CachingProxy.Data;
using CachingProxy.Services;
using Microsoft.EntityFrameworkCore;
using System.CommandLine.Builder;
using System.CommandLine;
using CachingProxy.Endpoints;
using System.CommandLine.Parsing;

var isEfTooling = AppDomain.CurrentDomain
    .GetAssemblies()
    .Any(a => a.FullName!.StartsWith("Microsoft.EntityFrameworkCore.Design"));

var builder = WebApplication.CreateBuilder();

builder.Services.AddHttpClient();
builder.Services.AddTransient<IProxyHandler, ProxyHandler>();
builder.Services.AddTransient<ICacheRepository, CacheRepository>();
builder.Services.AddDbContext<CacheContext>(options =>
{
    options.UseSqlite("Data Source=cache.db");
});

var app = builder.Build();

if (!isEfTooling)
{
    var portOption = new Option<int>(["--port", "-p"], "The port on which the caching proxy server will run.");
    var originOption = new Option<string>(["--origin", "-o"], "The URL of the server to which the requests will be forwarded.");
    var clearCacheOption = new Option<bool>(["--clear-cache", "-c"], "Clears the saved cache.");

    var rootCommand = new RootCommand("Starts a caching proxy server.")
    {
        portOption,
        originOption,
        clearCacheOption
    };

    rootCommand.AddValidator(result =>
    {
        var portSupplied = result.FindResultFor(portOption) is not null;
        var originSupplied = result.FindResultFor(originOption) is not null;
        
        if ((portSupplied || originSupplied) && !(portSupplied && originSupplied))
        {
            result.ErrorMessage = "--port and --origin must both be provided if one is specified.";
        }
    });

    rootCommand.SetHandler(async (int port, string origin, bool clearCache) =>
    {
        if (clearCache)
        {
            Console.WriteLine("Clearing the cache...");
            using var scope = app.Services.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<ICacheRepository>();
            await repo.ClearCacheAsync();
            Console.WriteLine("Cache cleared.");
        }

        if (port > 0 && !string.IsNullOrEmpty(origin))
        {
            Console.WriteLine($"Starting service on port {port} with origin {origin}");
            app.AddProxyEndpoints(origin);
            app.Run($"http://localhost:{port}");
        }
    }, portOption, originOption, clearCacheOption);

    var parser = new CommandLineBuilder(rootCommand)
        .UseDefaults()
        .Build();

    return await parser.InvokeAsync(args);
}
else
{
    // We're under EF tooling, return success
    return 0;
}
