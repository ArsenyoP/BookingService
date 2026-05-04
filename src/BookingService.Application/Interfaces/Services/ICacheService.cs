namespace Booking.Application.Interfaces.Services
{
    public interface ICacheService
    {
        public Task<T?> GetAsync<T>(string key, CancellationToken ct = default);
        public Task SetAsync<T>(string key, T value, TimeSpan expiration, CancellationToken ct = default);
        public Task RemoveAsync(string key, CancellationToken ct = default);
    }
}
