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
    private string _connectionString = default!;

    public IntegrationTestWebAppFactory()
    {
        // 1. Фіксимо JWT конфігурацію для GitHub Actions прямо в процесі
        Environment.SetEnvironmentVariable("JWT__Secret", "SuperSecretKeyThatIsLongEnoughToSatisfyJwtRequirements123!");
        Environment.SetEnvironmentVariable("JWT__Issuer", "BookingService");
        Environment.SetEnvironmentVariable("JWT__Audience", "BookingService");

        // 2. Ініціалізуємо та запускаємо контейнер БД синхронно, 
        // щоб рядок підключення був готовий ДО ConfigureWebHost
        _dbContainer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword("Password123!")
            .Build();

        _dbContainer.StartAsync().GetAwaiter().GetResult();

        _connectionString = _dbContainer.GetConnectionString()
            .Replace("Database=master", "Database=BookingTestDb");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // Заміна DbContext
            var descriptors = services.Where(d =>
                d.ServiceType == typeof(DbContextOptions<AppDbContext>) ||
                d.ServiceType == typeof(AppDbContext) ||
                d.ServiceType.Name.Contains("DbContextPool")).ToList();

            foreach (var descriptor in descriptors)
                services.Remove(descriptor);

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(_connectionString));

            // Заміна Dapper Queries
            var queriesDescriptorBooking = services.FirstOrDefault(d => d.ServiceType == typeof(IBookingQueries));
            if (queriesDescriptorBooking != null) services.Remove(queriesDescriptorBooking);
            services.AddScoped<IBookingQueries>(sp => new BookingQueries(_connectionString));

            var queriesDescriptorRoom = services.FirstOrDefault(d => d.ServiceType == typeof(IRoomQueries));
            if (queriesDescriptorRoom != null) services.Remove(queriesDescriptorRoom);
            services.AddScoped<IRoomQueries>(sp => new RoomQueries(_connectionString));

            var queriesDescriptorReview = services.FirstOrDefault(d => d.ServiceType == typeof(IReviewQueries));
            if (queriesDescriptorReview != null) services.Remove(queriesDescriptorReview);
            services.AddScoped<IReviewQueries>(sp => new ReviewQueries(_connectionString));

            // Заміна Redis на вбудований MemoryCache, щоб тести не шукали реальний Redis інстанс
            var redisDescriptor = services.FirstOrDefault(d =>
                d.ServiceType.FullName != null && d.ServiceType.FullName.Contains("StackExchangeRedis"));
            if (redisDescriptor != null) services.Remove(redisDescriptor);

            services.AddDistributedMemoryCache();
        });
    }

    public async Task InitializeAsync()
    {
        // Створення схеми БД
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.EnsureCreatedAsync();

        // Ініціалізація Respawner для очищення між тестами
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