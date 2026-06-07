using Booking.API;
using Booking.Application.Interfaces.IQueries;
using Booking.Application.Queries;
using Booking.Infrastructure.Data;
using Booking.Infrastructure.Queries;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Respawn;
using System.Data.Common;
using Testcontainers.MsSql;

namespace BookingService.IntegrationTests;

public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _dbContainer;
    private DbConnection _dbConnection = default!;
    private Respawner _respawner = default!;
    private string _connectionString = default!;  // <-- збережи рядок підключення

    public IntegrationTestWebAppFactory()
    {
        _dbContainer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword("Password123!")
            .Build();

        _dbContainer.StartAsync().GetAwaiter().GetResult();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "JWT:SigningKey", "SuperSecretKeyThatIsLongEnoughToSatisvevmpemavmacxj340amcgxgghe9bivegsbocsovoooonfyJwtRequirements123!" },
                { "JWT:Issuer", "BookingService" },
                { "JWT:Audience", "BookingService" }
            });
        });

        builder.ConfigureTestServices(services =>
        {
            // Видаляємо DbContext
            var descriptors = services.Where(d =>
                d.ServiceType == typeof(DbContextOptions<AppDbContext>) ||
                d.ServiceType == typeof(AppDbContext) ||
                d.ServiceType.Name.Contains("DbContextPool")).ToList();

            foreach (var descriptor in descriptors)
                services.Remove(descriptor);

            _connectionString = _dbContainer.GetConnectionString()
                .Replace("Database=master", "Database=BookingTestDb");

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(_connectionString));

            // Queries
            var queriesDescriptorBooking = services.FirstOrDefault(d =>
                d.ServiceType == typeof(IBookingQueries));
            if (queriesDescriptorBooking != null)
                services.Remove(queriesDescriptorBooking);
            services.AddScoped<IBookingQueries>(sp => new BookingQueries(_connectionString));

            var queriesDescriptorRoom = services.FirstOrDefault(d =>
                d.ServiceType == typeof(IRoomQueries));
            if (queriesDescriptorRoom != null)
                services.Remove(queriesDescriptorRoom);
            services.AddScoped<IRoomQueries>(sp => new RoomQueries(_connectionString));

            var queriesDescriptorReview = services.FirstOrDefault(d =>
                d.ServiceType == typeof(IReviewQueries));
            if (queriesDescriptorReview != null)
                services.Remove(queriesDescriptorReview);
            services.AddScoped<IReviewQueries>(sp => new ReviewQueries(_connectionString));

            // Видаляємо ВСЕ пов'язане з Redis
            RemoveRedisServices(services);

            // Замінюємо на in-memory кеш
            services.AddDistributedMemoryCache();
        });
    }

    private static void RemoveRedisServices(IServiceCollection services)
    {
        var toRemove = services
        .Where(d =>
        {
            var typeName = d.ServiceType.FullName ?? string.Empty;
            var implName = d.ImplementationType?.FullName ?? string.Empty;

            return typeName.Contains("Redis", StringComparison.OrdinalIgnoreCase)
                || implName.Contains("Redis", StringComparison.OrdinalIgnoreCase)
                || d.ServiceType == typeof(IDistributedCache);
        })
        .ToList();

        foreach (var descriptor in toRemove)
            services.Remove(descriptor);

        // Замінюємо distributed cache
        services.AddDistributedMemoryCache();

        // Замінюємо output cache (без Redis)
        services.AddOutputCache();
    }

    public async Task InitializeAsync()
    {
        // Тільки для застосування міграцій/схеми
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.EnsureCreatedAsync();

        // Пряме з'єднання — не залежить від DbContext або scope
        _dbConnection = new SqlConnection(_connectionString);
        await _dbConnection.OpenAsync();

        _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.SqlServer,
            TablesToIgnore = ["__EFMigrationsHistory", "AspNetRoles"]
        });
    }

    public async Task ResetDatabaseAsync()
    {
        await _respawner.ResetAsync(_dbConnection);
    }

    public new async Task DisposeAsync()
    {
        await _dbConnection.DisposeAsync();
        await _dbContainer.StopAsync();
    }
}