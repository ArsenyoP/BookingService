using Booking.Infrastructure.Data;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace BookingService.IntegrationTests
{
    public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>, IAsyncLifetime
    {
        private readonly IServiceScope _scope;
        protected readonly ISender Sender;
        protected readonly AppDbContext DbContext;
        private readonly IntegrationTestWebAppFactory _factory;

        protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
        {
            _factory = factory;
            _scope = factory.Services.CreateScope();

            Sender = _scope.ServiceProvider.GetRequiredService<ISender>();

            DbContext = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
        }

        public Task InitializeAsync() => Task.CompletedTask;

        // Викликається ОДРАЗУ ПІСЛЯ кожного тесту
        public async Task DisposeAsync()
        {
            _scope.Dispose(); // Спочатку закриваємо наш скоуп тестів
            await _factory.ResetDatabaseAsync(); // Викликаємо очищення Respawn!
        }
    }
}
