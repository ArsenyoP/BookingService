using Booking.API.Endpoints;
using Booking.Application;
using Booking.Infrastructure;
using Booking.Infrastructure.Data;
using Booking.Infrastructure.ExtensionMethods;
using Booking.Infrastructure.Seeding;
using HealthChecks.UI.Client;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Text.Json.Serialization;

namespace Booking.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            });

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .CreateLogger();

            builder.Host.UseSerilog();

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });


            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen();

            builder.Services.AddApplication();
            builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);
            builder.Services.AddRateLimiting();
            builder.Services.AddPresentation();



            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                if (app.Environment.IsDevelopment())
                {
                    using var scope = app.Services.CreateScope();

                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    Log.Information("Applying pending database migrations...");
                    await dbContext.Database.MigrateAsync();
                    Log.Information("Database migrations applied successfully.");

                    // 2. Òâ³é ³ñíóþ÷èé ñ³äèíã äàíèõ
                    var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
                    await seeder.SeedAsync();
                }
            }

            app.MapHealthChecks("health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseExceptionHandler();
            app.UseSerilogRequestLogging();
            app.UseRateLimiter();
            //app.UseHttpsRedirection();
            app.UseOutputCache();
            app.UseAuthentication();
            app.UseAuthorization();
            //app.MapControllers();

            app.MapAuthEndpoints();
            app.MapAmenityEndpoints();
            app.MapBookingEndpoints();
            app.Run();
        }
    }
}
