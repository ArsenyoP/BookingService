using Booking.Domain.Entities;
using Booking.Domain.Enums;
using Booking.Domain.ValueObjects;
using Booking.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Booking.Infrastructure.Seeding
{
    public sealed class DataSeeder(ILogger<DataSeeder> _logger,
        AppDbContext _dbContext,
        UserManager<User> _userManager,
        IConfiguration _configuration)
    {
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public async Task SeedAsync(CancellationToken ct = default)
        {
            var seedPath = _configuration["SeedData:Path"] ?? "Seeds";

            _logger.LogInformation("Starting data seeding from path: {Path}", seedPath);

            await SeedListingsAsync(seedPath, ct);

            _logger.LogInformation("Data seeding completed");
        }

        private async Task SeedListingsAsync(string SeedPath, CancellationToken ct)
        {
            var filePath = Path.Combine(SeedPath, "listings.json");
            if (!File.Exists(filePath))
            {
                _logger.LogWarning("listings.json not found at {Path}, skipping", filePath);
                return;
            }

            var json = await File.ReadAllTextAsync(filePath, ct);
            var models = JsonSerializer.Deserialize<List<ListingSeedingModel>>(json, _jsonOptions);

            if (models is null || models.Count == 0)
                return;

            var existingTitles = await _dbContext.Listings
               .Select(l => l.Title)
               .ToHashSetAsync();

            var toCreate = models.Where(m => !existingTitles.Contains(m.Title)).ToList();

            if (toCreate.Count == 0)
            {
                _logger.LogInformation("All listings already exist, skipping");
                return;
            }

            foreach (var model in toCreate)
            {
                var listingType = Enum.Parse<ListingType>(model.ListingType);

                var addressResult = Address.Create(model.Country, model.City, model.Street, model.HouseNumber, model.Floor);
                if (!addressResult.IsSuccess)
                {
                    _logger.LogWarning("Failed to create address for listing {Title}: {Error}",
                        model.Title, addressResult.Error.Description);
                    continue;
                }

                var listingResult = Listing.Create(model.Title, model.Description, addressResult.Value!, listingType);

                if (!listingResult.IsSuccess)
                {
                    _logger.LogWarning("Failed to create listing {Title}: {Error}",
                        model.Title, listingResult.Error.Description);
                    continue;
                }

                _dbContext.Listings.Add(listingResult.Value!);
            }

            await _dbContext.SaveChangesAsync(ct);
        }
    }
}
