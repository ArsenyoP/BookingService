# Caching and Cache Invalidation in BookingService

This document explains the caching mechanisms and cache invalidation strategies implemented in the BookingService project.

## Overview

The BookingService implements multiple layers of caching to improve performance and reduce load on external services and databases:

1. **Distributed Caching** - Using Redis via `ICacheService` for caching arbitrary data
2. **Query Caching** - Automatic caching of query results using MediatR pipeline behaviors
3. **Cache Invalidation** - Automatic removal of cached data when related data is modified
4. **Output Caching** - HTTP response caching for GET endpoints using ASP.NET Core OutputCache

## Distributed Caching (ICacheService)

The `ICacheService` interface (implemented by `CacheService`) provides a generic way to cache and retrieve data using Redis as the backing store.

### Interface Definition
```csharp
public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken ct = default);
    Task SetAsync<T>(string key, T value, TimeSpan expiration, CancellationToken ct = default);
    Task RemoveAsync(string key, CancellationToken ct = default);
}
```

### Implementation Details
- Uses `IDistributedCache` (Redis) under the hood
- Serializes/deserializes objects using `System.Text.Json`
- Provides absolute expiration times (relative to now)
- Logs cache removals for debugging

### Registration
The service is registered as scoped in `Booking.Infrastructure.DependencyInjection.cs`:
```csharp
services.AddScoped<ICacheService, CacheService>();
```

### Redis Configuration
Redis is configured in the Infrastructure layer:
```csharp
services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = configuration["Redis:Connection"];
    options.InstanceName = "Distributed_Cache_";
});
```

## Query Caching Behavior

The `CachingBehavior<TRequest, TResponse>` automatically caches the results of queries that implement the `ICachableQuery` interface.

### How It Works
1. When a query is processed through MediatR, the caching behavior checks if it implements `ICachableQuery`
2. If it does, it attempts to retrieve the result from cache using the query's `Key`
3. If found in cache, it returns the cached result immediately
4. If not found, it executes the query handler, then caches the result using the query's `Key` and `Expiration`

### ICachableQuery Interface
```csharp
public interface ICachableQuery
{
    string Key { get; }
    TimeSpan Expiration { get; }
}
```

### Example Query Implementation
```csharp
public sealed record GetByIdQuery(Guid id) : IQuery<ListingResponseDto>, ICachableQuery
{
    public string Key => $"listing:{id}";
    public TimeSpan Expiration => TimeSpan.FromMinutes(5);
}
```

### Registration
The caching behavior is registered in the MediatR pipeline in `Booking.Application.DependencyInjection.cs`:
```csharp
config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
```

## Cache Invalidation Behavior

The `CacheInvalidationBehavior<TRequest, TResponse>` automatically removes cached data when commands that implement `ICacheInvalidationCommand` are executed successfully.

### How It Works
1. When a command is processed through MediatR, the cache invalidation behavior executes the command handler first
2. If the command was successful (returns a successful result), it removes the cache entry associated with the command's `Key`
3. This ensures that cached data is invalidated when the underlying data changes

### ICacheInvalidationCommand Interface
```csharp
public interface ICacheInvalidationCommand
{
    string Key { get; }
}
```

### Example Command Implementation
```csharp
public sealed record DeleteListingCommand(Guid ListingId) : ICommand<Guid>, ICacheInvalidationCommand
{
    public string Key => $"listing:{ListingId}";
}
```

### Registration
The cache invalidation behavior is registered in the MediatR pipeline in `Booking.Application.DependencyInjection.cs`:
```csharp
config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(CacheInvalidationBehavior<,>));
```

## Output Caching (HTTP Response Caching)

The project uses ASP.NET Core's built-in output caching middleware to cache HTTP responses for GET endpoints.

### Middleware Registration
In `Booking.API.Program.cs`:
```csharp
app.UseOutputCache();
```

### Custom Output Caching Policy
The `CustomOutputCachingPolicy` class implements `IOutputCachePolicy` to customize caching behavior:

#### Key Features:
- Only caches GET requests
- Varies cache by all query parameters (`context.CacheVaryByRules.QueryKeys = "*"`)
- Adds tags based on route values (e.g., `target_{targetId}`)
- Prevents caching when:
  - Response contains Set-Cookie headers
  - HTTP status code is not 200 OK

### Redis Configuration for Output Caching
```csharp
services.AddStackExchangeRedisOutputCache(options =>
{
    options.Configuration = configuration["Redis:Connection"];
    options.InstanceName = "Output_Cache_";
});
```

## Cache Key Conventions

The project uses consistent key naming conventions for cache entries:

### For Entity Caching
```
{entityType}:{entityId}
```
Examples:
- `listing:{guid}`
- `room:{guid}`
- `booking:{guid}`

### For Query-Based Caching
Keys are defined explicitly in the `ICachableQuery.Key` property, typically following the same convention.

## Expiration Policies

### Distributed Cache (ICacheService)
- Expiration is specified per cache entry when calling `SetAsync`
- Query caching behaviors use the `Expiration` property from `ICachableQuery`
- Common expiration time: 5 minutes (as seen in example queries)

### Output Caching
- Uses ASP.NET Core's default cache duration unless specified otherwise
- Can be customized per endpoint using `[OutputCache]` attributes if needed

## Cache Invalidation Triggers

Cached data is automatically invalidated when:
1. **Delete Operations** - Commands like `DeleteListingCommand`, `DeleteRoomCommand`, etc. implement `ICacheInvalidationCommand` and trigger removal of the specific entity's cache entry
2. **Update Operations** - While not shown in the current codebase, similar patterns could be applied for update commands
3. **Create Operations** - Typically don't require invalidation of existing entries, but might necessitate invalidation of list queries (not currently implemented)

## Implementation Files

- `src/BookingService.Infrastructure/Services/CacheService.cs` - Redis cache service implementation
- `src/BookingService.Application/Behaviors/CachingBehavior.cs` - Query caching behavior
- `src/BookingService.Application/Behaviors/CacheInvalidationBehavior.cs` - Cache invalidation behavior
- `src/BookingService.Application/Abstractions/ICachableQuery.cs` - Interface for cacheable queries
- `src/BookingService.Application/Abstractions/ICacheInvalidationCommand.cs` - Interface for cache invalidating commands
- `src/BookingService.API/OutputCaching/CustomOutputCachingPolicy.cs` - Custom output caching policy
- `src/BookingService.Infrastructure/DependencyInjection.cs` - Redis cache and output cache registration
- `src/BookingService.Application/DependencyInjection.cs` - MediatR behavior registration

## Usage Guidelines

### Adding Caching to a New Query
1. Make your query implement `ICachableQuery`
2. Provide a unique `Key` property (typically based on query parameters)
3. Set an appropriate `Expiration` TimeSpan
4. The caching behavior will automatically cache results

### Adding Cache Invalidation to a New Command
1. Make your command implement `ICacheInvalidationCommand`
2. Provide a `Key` property that matches the cache key used for the corresponding query
3. The cache invalidation behavior will automatically remove the cache entry on successful command execution

### Customizing Output Caching
1. The `CustomOutputCachingPolicy` applies globally to all endpoints
2. To customize per endpoint, use `[OutputCache]` attributes with specific policies
3. Modify `CustomOutputCachingPolicy` if global caching rules need to change

## Cached Items Summary

The following table summarizes what is being cached in the BookingService application and the type of cache used:

| What is Cached | Cache Type | Implementation Details | Key Features |
|----------------|------------|------------------------|--------------|
| Query Results (GetById queries for listings, rooms, bookings) | Distributed Cache (Redis) | `CachingBehavior<TRequest, TResponse>` + `ICacheService` | Keys like `listing:{id}`, `room:{id}`, `booking:{id}`; 5-minute expiration |
| Individual Entity Data (manual caching) | Distributed Cache (Redis) | Direct `ICacheService` usage | Generic caching service for any serializable data |
| HTTP GET Responses (Review endpoints) | Output Cache (Redis-backed) | ASP.NET Core OutputCache + `CustomOutputCachingPolicy` | Caches GET responses; varies by query params; tags by targetId; excludes responses with cookies or non-200 status |
| Public Reviews List | Output Cache (Redis-backed) | Named policy "PublicReviews" in `AddPresentation` | Specific to reviews endpoints; 10-minute expiration; cache key prefix "reviews_" |

### Detailed Breakdown

#### 1. Query Caching Behavior
- **What**: Results of queries implementing `ICachableQuery` (primarily GetById queries)
- **Where**: `src/BookingService.Application/Behaviors/CachingBehavior.cs`
- **How**: Uses MediatR pipeline behavior to intercept queries, check cache via `ICacheService`, and store results
- **Examples**: 
  - `GetByIdQuery` for listings (`src/BookingService.Application/UseCases/Listing/GetById/GetByIdQuery.cs`)
  - Similar patterns exist for rooms and bookings

#### 2. Distributed Caching Service (ICacheService)
- **What**: Arbitrary serializable data
- **Where**: `src/BookingService.Infrastructure/Services/CacheService.cs`
- **How**: Wrapper around `IDistributedCache` (Redis) with JSON serialization
- **Usage**: Can be injected anywhere for manual caching needs

#### 3. HTTP Response Output Caching
- **What**: HTTP GET response bodies
- **Where**: 
  - Global: `src/BookingService.API/OutputCaching/CustomOutputCachingPolicy.cs`
  - Named policies: `src/BookingService.API/DependencyInjection.cs`
  - Applied: `src/BookingService.API/Controllers/ReviewControllers.cs`
- **How**: ASP.NET Core's `OutputCache` middleware backed by Redis
- **Policies**:
  - **Global Policy** (`CustomOutputCachingPolicy`): Applies to all GET endpoints unless overridden
    - Only caches GET requests
    - Varies by all query parameters
    - Adds tags based on route values (e.g., `target_{targetId}`)
    - Prevents caching when response has Set-Cookie headers or non-200 status
  - **Named Policy** ("PublicReviews"): Specific to review endpoints
    - Uses `CustomOutputCachingPolicy`
    - Cache key prefix: "reviews_"
    - Expires after 10 minutes
    - Applied to `GetReviewsByTargetId` endpoint via `[OutputCache(PolicyName = "PublicReviews")]`

#### 4. Cache Invalidation Behavior
- **What**: Removes cached data when related data is modified
- **Where**: `src/BookingService.Application/Behaviors/CacheInvalidationBehavior.cs`
- **How**: MediatR pipeline behavior that removes cache entries after successful commands
- **Triggers**: Commands implementing `ICacheInvalidationCommand` (delete operations)
- **Examples**:
  - `DeleteListingCommand` removes `listing:{id}` cache entry
  - Similar patterns for deleting rooms, bookings, amenities

## Considerations and Limitations

1. **Cache Stampede** - The current implementation doesn't prevent cache stampede (multiple simultaneous cache misses for the same key). For high-traffic scenarios, consider implementing a locking mechanism.

2. **Complex Invalidation Scenarios** - The current invalidation is key-based and works well for direct entity operations. For operations that affect multiple entities (e.g., updating a listing might affect search results), additional invalidation logic may be needed.

3. **Memory Usage** - Monitor Redis memory usage as cached data grows. Consider implementing key eviction policies or setting appropriate expiration times.

4. **Cache Consistency** - In a distributed environment, ensure all instances share the same Redis cache to maintain consistency.

5. **Serialization** - The cache uses `System.Text.Json` for serialization. Ensure cached types are serializable and consider versioning if schemas change over time.