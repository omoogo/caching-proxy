# Caching Proxy App

![.NET](https://img.shields.io/badge/.NET-9.0-blue)

This project is a learning exercise based on the [Caching Proxy project](https://roadmap.sh/projects/caching-server) from [roadmap.sh](https://roadmap.sh/). It's a CLI application built with C# that forwards HTTP requests to a target server and caches the responses locally.

## Features

- Specify port and origin server URL to run the proxy locally.
- Command line option to clear the cache.
- Caches HTTP responses in a local SQLite database.
- Adds cache status headers (X-Cache: HIT or MISS).

## Project Structure

```
caching-proxy/
├── CachingProxy/                          # System.CommandLine-based CLI and proxy logic
│   ├── Data/                              # Data access layer for caching
│   │   ├── Entities/
│   │   │   └── CachedResponse.cs          # Entity model for cached HTTP responses
│   │   ├── Migrations/                    # EF Core migration files
│   │   ├── CacheDbContext.cs              # Entity Framework Core DbContext for SQLite
│   │   ├── CacheRepository.cs             # Concrete implementation of caching repository
│   │   └── ICacheRepository.cs            # Interface for cache repository
│   ├── Endpoints/
│   │   └── ProxyEndpoints.cs              # Minimal API endpoint routing to ProxyHandler
│   ├── Services/
│   │   └── ProxyHandler.cs                # Core logic: forwards requests and manages caching
│   ├── cache.db                           # SQLite database file storing cached responses
│   └── Program.cs                         # Application entry point
├── CachingProxy.UnitTests/                # Unit tests using xUnit
│   └── CacheRepositoryTests.cs            # Tests for caching logic in repository
├── .editorconfig                          # Code style configuration
├── CachingProxyApp.sln                    # Visual Studio solution file
└── README.md                              # Project documentation
```

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)

### Start the caching proxy server

```bash
dotnet run --project CachingProxy --port 3000 --origin https://dummyjson.com
```

### Clear the cache

```bash
dotnet run --project CachingProxy --clear-cache
```

### Run the Tests

```bash
dotnet test
```
