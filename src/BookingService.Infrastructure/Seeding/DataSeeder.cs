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
            await SeedRoomsAsync(seedPath, ct);
            await SeedUsersAsync(seedPath, ct);
            await SeedBookingAsync(seedPath, ct);
            await SeedAmenitiesAsync(seedPath, ct);

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

        private async Task SeedRoomsAsync(string SeedPath, CancellationToken ct)
        {
            var filePath = Path.Combine(SeedPath, "rooms.json");
            if (!File.Exists(filePath))
            {
                _logger.LogWarning("rooms.json not found at {Path}, skipping", filePath);
                return;
            }

            var json = await File.ReadAllTextAsync(filePath);
            var models = JsonSerializer.Deserialize<List<RoomSeedingModel>>(json, _jsonOptions);

            if (models is null || models.Count == 0)
                return;

            var listings = await _dbContext.Listings
                .ToDictionaryAsync(l => l.Title, ct);

            var existingRoomTitle = await _dbContext.Rooms
                .Select(r => r.Title)
                .ToHashSetAsync(ct);

            var toCreate = models.Where(m => !existingRoomTitle.Contains(m.Title)).ToList();

            if (toCreate.Count == 0)
            {
                _logger.LogInformation("All rooms already exist, skipping");
                return;
            }

            foreach (var model in toCreate)
            {
                if (!listings.TryGetValue(model.ListingTitle, out var listing))
                {
                    _logger.LogWarning(
                         "Listing '{ListingTitle}' not found for room '{RoomTitle}', skipping",
                         model.ListingTitle, model.Title);
                    continue;
                }

                if (!Enum.TryParse<RoomType>(model.Type, out var roomType))
                {
                    _logger.LogWarning("Unknown amenity category: {Category}", model.Type);
                    continue;
                }

                var roomResult = Room.Create(
                    model.Title,
                    model.Description,
                    roomType,
                    model.PricePerNight,
                    model.AdultsCapacity,
                    model.ChildrenCapacity,
                    listing!.Id);

                if (!roomResult.IsSuccess)
                {
                    _logger.LogWarning("Failed to create room {Title}: {Error}",
                        model.Title, roomResult.Error.Description);
                    continue;
                }

                _dbContext.Rooms.Add(roomResult.Value!);
                _logger.LogInformation("Seeding room: {Title} in {Listing}",
                    model.Title, model.ListingTitle);
            }

            await _dbContext.SaveChangesAsync(ct);
        }

        private async Task SeedUsersAsync(string SeedPath, CancellationToken ct)
        {
            var filePath = Path.Combine(SeedPath, "users.json");
            if (!File.Exists(filePath))
            {
                _logger.LogWarning("users.json not found at {Path}, skipping", filePath);
                return;
            }

            var json = File.ReadAllText(filePath);
            var models = JsonSerializer.Deserialize<List<UserSeedModel>>(json, _jsonOptions);

            if (models is null || models.Count == 0)
                return;

            foreach (var model in models)
            {
                var existing = await _userManager.FindByEmailAsync(model.Email);
                if (existing is not null)
                {
                    _logger.LogInformation("User {Email} already exists, skipping", model.Email);
                    continue;
                }

                var userResult = User.Create(
                    model.FirstName,
                    model.LastName,
                    model.DateOfBirth,
                    model.Email,
                    model.UserName);

                if (!userResult.IsSuccess)
                {
                    _logger.LogWarning("Failed to create user {Email}: {Error}",
                        model.Email, userResult.Error.Description);
                    continue;
                }

                var createResult = await _userManager.CreateAsync(userResult.Value!, model.Password);
                if (!createResult.Succeeded)
                {
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    _logger.LogWarning("Failed to create user {Email}: {Errors}", model.Email, errors);
                    continue;
                }

                var roleResult = await _userManager.AddToRoleAsync(userResult.Value!, model.Role);
                if (!roleResult.Succeeded)
                {
                    var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    _logger.LogWarning("Failed to assign role for {Email}: {Errors}", model.Email, errors);
                }

                _logger.LogInformation("Seeded user: {Email} with role {Role}", model.Email, model.Role);
            }
        }

        private async Task SeedBookingAsync(string SeedPath, CancellationToken ct)
        {
            var filePath = Path.Combine(SeedPath, "bookings.json");
            if (!File.Exists(filePath))
            {
                _logger.LogWarning("bookings.json not found at {Path}, skipping", filePath);
                return;
            }

            var json = File.ReadAllText(filePath);
            var models = JsonSerializer.Deserialize<List<BookingSeedingModel>>(json, _jsonOptions);

            if (models is null || models.Count == 0)
                return;

            var rooms = await _dbContext.Rooms.ToDictionaryAsync(r => r.Title, ct);
            var users = await _dbContext.Users.ToDictionaryAsync(u => u.UserName!, ct);

            var existingBookings = _dbContext.Bookings
                .Select(b => new { b.RoomId, b.GuestId, b.Period.StartDate })
                .AsEnumerable()
                .Select(b => (b.RoomId, b.GuestId, b.StartDate))
                .ToHashSet();

            var toCreate = models.Where(m =>
            {
                if (!rooms.TryGetValue(m.RoomTitle, out var room) ||
                    !users.TryGetValue(m.UserName, out var user))
                {
                    return false;
                }

                var key = (room.Id, user.Id, m.StartDate);

                var booking = existingBookings.Contains(key);

                if (booking)
                {
                    _logger.LogWarning("Booking for room: {roomTitle} for user: {user.Username} from: {startDate} allready exsists",
                        m.RoomTitle, m.UserName, m.StartDate);
                }

                return !booking;
            }).ToList();

            foreach (var model in toCreate)
            {
                if (!rooms.TryGetValue(model.RoomTitle, out var room))
                {
                    _logger.LogWarning("Can't get room with title: {roomTitle}", model.RoomTitle);
                    continue;
                }

                if (!users.TryGetValue(model.UserName, out var user))
                {
                    _logger.LogWarning("Can't get user with username: {userName}", model.UserName);
                    continue;
                }

                var periodResult = DateRange.Create(model.StartDate, model.EndDate);
                if (!periodResult.IsSuccess)
                    continue;

                var bookingResult = Bookings.Create(
                    periodResult.Value!,
                    model.NumberOfAdults,
                    model.NumberOfChildren,
                    room,
                    user);

                if (!bookingResult.IsSuccess)
                    continue;

                _dbContext.Bookings.Add(bookingResult.Value!);
                _logger.LogInformation("Seeding room: guest: {userName}, room: {roomTitle}, startDate: {startDate}",
                    user.UserName, room.Title, bookingResult.Value!.Period.StartDate);
            }
            await _dbContext.SaveChangesAsync(ct);
        }

        private async Task SeedAmenitiesAsync(string SeedPath, CancellationToken ct)
        {
            var filePath = Path.Combine(SeedPath, "amenities.json");
            if (!File.Exists(filePath))
            {
                _logger.LogWarning("amenities.json not found at {Path}, skipping", filePath);
                return;
            }

            var json = File.ReadAllText(filePath);
            var models = JsonSerializer.Deserialize<List<AmenitiesSeedingModel>>(json, _jsonOptions);

            if (models is null || models.Count == 0)
                return;

            var existingAmenities = await _dbContext.Amenities.Select(a => a.Name)
                .ToHashSetAsync();

            var toCreate = models.Where(a => !existingAmenities.Contains(a.AmenityName)).ToList();

            foreach (var model in toCreate)
            {
                if (!Enum.TryParse<AmenityCategory>(model.AmenityCategory, out var amenityCategory))
                {
                    _logger.LogWarning("Unknown amenity category: {Category}", model.AmenityCategory);
                    continue;
                }

                var amenityResult = Amenity.Create(model.AmenityName, amenityCategory);
                if (!amenityResult.IsSuccess)
                {
                    _logger.LogWarning("Can't create amenity with name: {amenityName}, error: {amenityError}",
                        amenityResult.Value!.Name, amenityResult.Error);
                    continue;
                }

                _dbContext.Amenities.Add(amenityResult.Value!);
                _logger.LogInformation("Seeding amenity: {amenityName} with category: {amenityCategory}",
                    amenityResult.Value!.Name, amenityResult.Value!.Category);
            }

            await _dbContext.SaveChangesAsync(ct);
        }
    }
}
