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

            var strategy = _dbContext.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);

                try
                {
                    await SeedListingsAsync(seedPath, ct);
                    await SeedRoomsAsync(seedPath, ct);
                    await SeedUsersAsync(seedPath, ct);
                    await SeedBookingAsync(seedPath, ct);
                    await SeedAmenitiesAsync(seedPath, ct);
                    await SeedAmenitiesToListingdAsync(seedPath, ct);
                    await SeedAmenitiesToRoomAsync(seedPath, ct);
                    await SeedReviewsAsync(seedPath, ct);

                    await transaction.CommitAsync(ct);
                    _logger.LogInformation("Data seeding completed");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Data seeding failed. All changes rolled back.");
                    //await transaction.RollbackAsync(ct);
                    throw;
                }
            });
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

        private async Task SeedAmenitiesToListingdAsync(string SeedPath, CancellationToken ct)
        {
            var filePath = Path.Combine(SeedPath, "listing_amenities.json");
            if (!File.Exists(filePath))
            {
                _logger.LogWarning("listing_amenities.json not found at {Path}, skipping", filePath);
                return;
            }

            var json = await File.ReadAllTextAsync(filePath, ct);
            var models = JsonSerializer.Deserialize<List<SeedToListingModel>>(json, _jsonOptions);

            if (models is null || models.Count == 0) return;

            var modelListingTitles = models.Select(m => m.ListingTitle).Distinct().ToList();
            var listingsMap = await _dbContext.Listings
                .Include(l => l.Amenities)
                .Where(l => modelListingTitles.Contains(l.Title))
                .ToDictionaryAsync(l => l.Title, ct);

            var modelAmenityNames = models.Select(m => m.AmenityName).Distinct().ToList();
            var amenitiesMap = await _dbContext.Amenities
                .Where(a => modelAmenityNames.Contains(a.Name))
                .ToDictionaryAsync(a => a.Name, ct);

            foreach (var model in models)
            {
                if (listingsMap.TryGetValue(model.ListingTitle, out var listing) &&
                    amenitiesMap.TryGetValue(model.AmenityName, out var amenity))
                {
                    if (!listing.Amenities.Any(a => a.Id == amenity.Id))
                    {
                        listing.AddAmenity(amenity);
                        _logger.LogInformation("Linked: {Amenity} -> {Listing}", amenity.Name, listing.Title);
                    }
                    else
                    {
                        _logger.LogWarning("Amenity: {amenity} allready added to {Listing}",
                            amenity.Name, listing.Title);
                        continue;
                    }
                }
                else
                {
                    _logger.LogWarning("Could not find Listing '{L}' or Amenity '{A}'",
                        model.ListingTitle, model.AmenityName);
                }
            }

            await _dbContext.SaveChangesAsync(ct);
        }

        private async Task SeedAmenitiesToRoomAsync(string SeedPath, CancellationToken ct)
        {
            var filePath = Path.Combine(SeedPath, "rooms_amenities.json");
            if (!File.Exists(filePath))
            {
                _logger.LogWarning("rooms_amenities.json not found at {Path}, skipping", filePath);
                return;
            }

            var json = await File.ReadAllTextAsync(filePath, ct);
            var models = JsonSerializer.Deserialize<List<SeedToRoomModel>>(json, _jsonOptions);

            if (models is null || models.Count == 0) return;

            //getting only amenities which name is in modelAmenityNames
            var modelAmenityNames = models.Select(m => m.AmenityName).Distinct().ToList();
            var modelAmenities = await _dbContext.Amenities
                .Where(a => modelAmenityNames.Contains(a.Name))
                .ToDictionaryAsync(a => a.Name, ct);

            var modelRoomTitles = models.Select(r => r.RoomTitle).Distinct().ToList();
            var modelRooms = await _dbContext.Rooms
                .Where(r => modelRoomTitles.Contains(r.Title))
                .Include(r => r.Amenities)
                .ToDictionaryAsync(r => r.Title, ct);

            foreach (var model in models)
            {
                if (modelAmenities.TryGetValue(model.AmenityName, out var amenity) &&
                    modelRooms.TryGetValue(model.RoomTitle, out var room))
                {
                    if (!room.Amenities.Any(a => a.Name == amenity.Name))
                    {
                        room.AddAmentity(amenity);
                        _logger.LogInformation("Linked: {Amenity} -> {Room}", amenity.Name, room.Title);
                    }
                    else
                    {
                        _logger.LogWarning("Amenity: {amenity} allready added to {Room}",
                            amenity.Name, room.Title);
                        continue;
                    }
                }
                else
                {
                    _logger.LogWarning("Could not find Room '{R}' or Amenity '{A}'",
                        model.RoomTitle, model.AmenityName);
                }
            }

            await _dbContext.SaveChangesAsync(ct);
        }

        private async Task SeedReviewsAsync(string SeedPath, CancellationToken ct)
        {
            var filePath = Path.Combine(SeedPath, "reviews.json");
            if (!File.Exists(filePath))
            {
                _logger.LogWarning("reviews.json not found at {Path}, skipping", filePath);
                return;
            }

            var json = File.ReadAllText(filePath);
            var models = JsonSerializer.Deserialize<List<SeedReviewsModel>>(json, _jsonOptions);

            if (models is null || models.Count == 0)
                return;

            var userNames = models.Select(x => x.UserName).Distinct().ToList();

            var users = await _dbContext.Users
                .Where(x => userNames.Contains(x.UserName!))
                .ToDictionaryAsync(x => x.UserName!, x => x.Id, ct);


            var roomTitles = models
                .Where(x => x.TargetType == "Room")
                .Select(x => x.TargetTitle).Distinct().ToList();

            var listingTitles = models
                .Where(x => x.TargetType == "Listing")
                .Select(x => x.TargetTitle).Distinct().ToList();


            var roomEntities = await _dbContext.Rooms
                .Where(x => roomTitles.Contains(x.Title))
                .ToDictionaryAsync(x => x.Title, ct);

            var listingEntities = await _dbContext.Listings
                .Where(x => listingTitles.Contains(x.Title))
                .ToDictionaryAsync(x => x.Title, ct);


            //Id's of users who is in JSON
            var userIds = users
                .Select(x => x.Value)
                .ToList();

            var existingReviews = await _dbContext.Review
                 .Where(r => userIds.Contains(r.UserId))
                 .Select(r => new { r.UserId, r.TargetId })
                 .ToHashSetAsync(ct);

            foreach (var model in models)
            {
                if (!users.TryGetValue(model.UserName, out var userId))
                {
                    _logger.LogWarning("User {UserName} not found, skipping review", model.UserName);
                    continue;
                }

                Guid targetId = Guid.Empty;
                ReviewsTargetType targetType;

                Room? roomEntity = null;
                Listing? listingEntity = null;

                if (model.TargetType == "Room" && roomEntities.TryGetValue(model.TargetTitle, out var room))
                {
                    targetId = room.Id;
                    targetType = ReviewsTargetType.Room;
                    roomEntity = room;
                }
                else if (model.TargetType == "Listing" && listingEntities.TryGetValue(model.TargetTitle, out var listing))
                {
                    targetId = listing.Id;
                    targetType = ReviewsTargetType.Listing;
                    listingEntity = listing;
                }
                else
                {
                    _logger.LogWarning("Target '{TargetTitle}' of type '{TargetType}' not found, skipping", model.TargetTitle, model.TargetType);
                    continue;
                }


                //checks for duplicates
                var reviewKey = new { UserId = userId, TargetId = targetId };

                if (existingReviews.Contains(reviewKey))
                {
                    _logger.LogWarning("Review from user {UserName} for target {TargetTitle} already exists, skipping", model.UserName, model.TargetTitle);
                    continue;
                }


                var reviewResult = Review.Create(userId, targetId, targetType, model.Score, model.Text);

                if (!reviewResult.IsSuccess)
                {
                    _logger.LogWarning("Failed to create review for {TargetTitle}: {Error}", model.TargetTitle, reviewResult.Error.Description);
                    continue;
                }

                if (roomEntity != null)
                {
                    roomEntity.UpdateRating(reviewResult.Value!.Score);
                }
                else if (listingEntity != null)
                {
                    listingEntity.UpdateRating(reviewResult.Value!.Score);
                }

                _dbContext.Review.Add(reviewResult.Value!);
                _logger.LogInformation("Seeded review from {User} to {Target}", model.UserName, model.TargetTitle);
            }

            await _dbContext.SaveChangesAsync(ct);

        }
    }
}
