using Booking.Application.Interfaces.Services;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Booking.Infrastructure.Services
{
    public class CacheService(ILogger<CacheService> _logger,
        IDistributedCache _cache) : ICacheService
    {
        public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
        {
            var value = await _cache.GetStringAsync(key, ct);

            if (value is null)
                return default;

            var result = JsonSerializer.Deserialize<T>(value);

            _logger.LogInformation("Getted {value} from cache", value);

            return result;
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan expiration, CancellationToken ct = default)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration
            };

            var json = JsonSerializer.Serialize(value);

            await _cache.SetStringAsync(key, json, options, ct);
            _logger.LogInformation("Setted {value} to cache", value);
        }

        public async Task RemoveAsync(string key, CancellationToken ct = default)
        {
            await _cache.RemoveAsync(key, ct);
            _logger.LogInformation("Removed value with key: {key} from cache", key);

        }
    }
}
