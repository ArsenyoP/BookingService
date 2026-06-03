using Booking.API;
using Booking.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;

namespace BookingService.IntegrationTests;

public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _dbContainer;

    public IntegrationTestWebAppFactory()
    {
        // Створюємо та жорстко запускаємо контейнер в конструкторі
        _dbContainer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
            .Build();

        _dbContainer.StartAsync().GetAwaiter().GetResult();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // 1. НАДІЙНО ВИДАЛЯЄМО ВСІ СТАРІ РЕЄСТРАЦІЇ (включаючи звичайний DbContext та DbContextPool)
            var descriptors = services.Where(d =>
                d.ServiceType == typeof(DbContextOptions<AppDbContext>) ||
                d.ServiceType == typeof(AppDbContext) ||
                d.ServiceType.Name.Contains("DbContextPool")).ToList();

            foreach (var descriptor in descriptors)
            {
                services.Remove(descriptor);
            }

            // 2. ПІДМІНЯЄМО НАЗВУ БАЗИ ДАНИХ З master НА ТЕСТОВУ
            // Це змусить EF Core створити нову чисту базу разом з таблицями
            var connectionString = _dbContainer.GetConnectionString()
                .Replace("Database=master", "Database=BookingTestDb");

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });
        });
    }

    public async Task InitializeAsync()
    {
        // 3. Створюємо scope і викликаємо створення структури
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Оскільки BookingTestDb ще не існує, цей метод створить її та НАКОТИТЬ УСІ ТАБЛИЦІ
        await dbContext.Database.EnsureCreatedAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
    }
}