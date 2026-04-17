using Booking.Application.DTOs.Amenities;
using Booking.Application.DTOs.Rooms;
using Booking.Application.Queries;
using Booking.Domain.Entities;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Queries
{
    public class RoomQueries(string connectionString) : IRoomQueries
    {
        public async Task<IReadOnlyList<RoomResponseDto>> GetAllPagedAsync(int page, int pageSize, CancellationToken ct = default)
        {
            using var connection = new SqlConnection(connectionString);
            var offset = (page - 1) * pageSize;

            const string sql = @"
                SELECT 
                    r.Id, 
                    r.Title, 
                    r.Description, 
                    r.Type, 
                    r.PricePerNight, 
                    r.AdultsCapacity,  
                    r.ChildrenCapacity, 
                    l.Title AS ListingTitle, 
                    r.ListingId
                FROM Rooms r
                INNER JOIN Listings l ON r.ListingId = l.Id
                ORDER BY r.Title
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var command = new CommandDefinition(
                sql,
                new { Offset = offset, PageSize = pageSize },
                cancellationToken: ct);

            var result = await connection.QueryAsync<RoomResponseDto>(command);

            return result.ToList().AsReadOnly();
        }

        public async Task<RoomResponseDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            using var connection = new SqlConnection(connectionString);
            const string sql = """
                SELECT 
                    r.Id,
                    r.Title,
                    r.Description,
                    r.Type,
                    r.PricePerNight,
                    r.AdultsCapacity,
                    r.ChildrenCapacity,
                    r.ListingId,
                    l.Title        AS ListingTitle,
                    a.Id           AS AmenityId,
                    a.Name         AS Name,
                    a.Category     AS Category
                FROM Rooms r
                INNER JOIN Listings l     ON l.Id = r.ListingId
                LEFT JOIN  RoomAmenities ra ON ra.RoomId = r.Id
                LEFT JOIN  Amenities a    ON a.Id = ra.AmenitiesId
                WHERE r.Id = @Id
                """;

            RoomResponseDto? result = null;

            await connection.QueryAsync<RoomResponseDto, AmenityDto?, RoomResponseDto>(
                sql,
                (room, amenity) =>
                {
                    if (result == null)
                    {
                        result = room;
                    }

                    if (amenity is not null && amenity.AmenityId != Guid.Empty)
                    {
                        result.Amenities.Add(amenity);
                    }

                    return room;
                },
                new { Id = id },
                splitOn: "AmenityId"
            );

            return result;
        }

        public async Task<IReadOnlyList<RoomResponseDto>> GetByListingIdAsync(
    Guid listingId,
    int page,
    int pageSize,
    CancellationToken ct = default)
        {
            using var connection = new SqlConnection(connectionString);
            var offset = (page - 1) * pageSize;

            const string sql = """
                SELECT
                    r.Id, 
                    r.Title, 
                    r.Description, 
                    r.Type, 
                    r.PricePerNight, 
                    r.AdultsCapacity, 
                    r.ChildrenCapacity, 
                    l.Title AS ListingTitle, 
                    r.ListingId,
                    a.Id       AS AmenityId,
                    a.Name     AS Name,
                    a.Category AS Category
                FROM (
                    SELECT r2.Id
                    FROM Rooms r2
                    WHERE r2.ListingId = @ListingId
                    ORDER BY r2.Title, r2.Id
                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY
                ) AS paged
                INNER JOIN Rooms r        ON r.Id = paged.Id
                INNER JOIN Listings l     ON l.Id = r.ListingId
                LEFT JOIN  RoomAmenities ra ON ra.RoomId = r.Id
                LEFT JOIN  Amenities a    ON a.Id = ra.AmenitiesId
                ORDER BY r.Title, r.Id;
                """;

            var roomDictionary = new Dictionary<Guid, RoomResponseDto>();

            await connection.QueryAsync<RoomResponseDto, AmenityDto?, RoomResponseDto>(
                sql,
                (room, amenity) =>
                {
                    if (!roomDictionary.TryGetValue(room.Id, out var existingRoom))
                    {
                        existingRoom = room with { Amenities = new List<AmenityDto>() };
                        roomDictionary[room.Id] = existingRoom;
                    }

                    if (amenity is not null)
                        existingRoom.Amenities.Add(amenity);

                    return existingRoom;
                },
                new { ListingId = listingId, Offset = offset, PageSize = pageSize },
                splitOn: "AmenityId"
            );

            return roomDictionary.Values.ToList();
        }

        public async Task<Room?> GetEntityByIdAsync(Guid id, CancellationToken ct = default)
        {
            using var connection = new SqlConnection(connectionString);
            const string sql = @"SELECT *
                FROM Rooms r 
                WHERE r.Id=@Id";

            var command = new CommandDefinition(
                sql,
                new { Id = id },
                cancellationToken: ct);

            var result = await connection.QueryFirstOrDefaultAsync<Room>(command);

            return result;
        }

        public async Task<IReadOnlyList<RoomResponseDto>> GetAllWithListingTitleAsync(
            int page, int pageSize,
            List<string>? amenityNames = null,
            CancellationToken ct = default)
        {
            var connection = new SqlConnection(connectionString);
            var offset = (page - 1) * pageSize;
            var namesCount = amenityNames?.Count ?? 0;
            var names = amenityNames ?? new List<string>();

            //TODO: Filters
            const string sql = """
                    SELECT 
                        r.Id,
                        r.Title,
                        r.Description,
                        r.Type,
                        r.PricePerNight,
                        r.AdultsCapacity,
                        r.ChildrenCapacity,
                        r.ListingId,
                        l.Title        AS ListingTitle,
                        a.Id           AS AmenityId,
                        a.Name         AS Name,
                        a.Category     AS Category
                    FROM (
                        SELECT r2.Id FROM Rooms r2
                        WHERE (@NamesCount = 0 OR (
                            SELECT COUNT(DISTINCT a2.Name)
                            FROM RoomAmenities ra2
                            INNER JOIN Amenities a2 ON ra2.AmenitiesId = a2.Id
                            WHERE ra2.RoomId = r2.Id AND a2.Name IN @Names
                        ) = @NamesCount)
                        ORDER BY r2.Title
                        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY
                    ) AS paged
                    INNER JOIN Rooms r        ON r.Id = paged.Id
                    INNER JOIN Listings l     ON l.Id = r.ListingId
                    LEFT JOIN  RoomAmenities ra ON ra.RoomId = r.Id
                    LEFT JOIN  Amenities a    ON a.Id = ra.AmenitiesId
                    ORDER BY r.Title;
                    """;

            var roomDictionary = new Dictionary<Guid, RoomResponseDto>();

            await connection.QueryAsync<RoomResponseDto, AmenityDto?, RoomResponseDto>(
                sql,
                (room, amenity) =>
                {
                    if (!roomDictionary.TryGetValue(room.Id, out var existingRoom))
                    {
                        existingRoom = room with { Amenities = new List<AmenityDto>() };
                        roomDictionary[room.Id] = existingRoom;
                    }

                    if (amenity is not null)
                        existingRoom.Amenities.Add(amenity);

                    return existingRoom;
                },
                new
                {
                    Offset = offset,
                    PageSize = pageSize,
                    Names = names,
                    NamesCount = namesCount
                },
                splitOn: "AmenityId"
            );

            return roomDictionary.Values.ToList();
        }
    }
}
