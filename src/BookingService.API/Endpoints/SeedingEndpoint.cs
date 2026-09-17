using Booking.Infrastructure.Seeding;

namespace Booking.API.Endpoints
{
    public static class SeedingEndpoint
    {
        public static void MapSeedingEndpoint(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/seeding")
                .RequireRateLimiting("write-limiter")
                .WithTags("Seeding");

            group.MapPost("/", async (DataSeeder seeder, CancellationToken ct) =>
            {
                await seeder.SeedAsync(ct);
                return Results.Ok(new { message = "Database seeded successfully." });
            });
        }
    }
}
