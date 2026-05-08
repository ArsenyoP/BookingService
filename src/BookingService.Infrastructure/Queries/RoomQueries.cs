using Booking.Application.DTOs.Amenities;
using Booking.Application.DTOs.Rooms;
using Booking.Application.Helpers.Room;
using Booking.Application.Queries;
using Booking.Domain.Entities;
using Booking.Domain.Enums;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Booking.Infrastructure.Queries
{
    public class RoomQueries(string connectionString) : IRoomQueries
    {

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

        public async Task<IReadOnlyList<RoomResponseDto>> GetAllWithAmenitiesAsync(RoomQueryObject filter, CancellationToken ct = default)
        {
            using var connection = new SqlConnection(connectionString);
            var offset = (filter.Page - 1) * filter.PageSize;

            var innerConditions = new List<string>();
            var parameters = new DynamicParameters();
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", filter.PageSize);

            if (filter.MinPrice.HasValue)
            {
                innerConditions.Add("r2.PricePerNight >= @MinPrice");
                parameters.Add("MinPrice", filter.MinPrice.Value);
            }

            if (filter.MaxPrice.HasValue)
            {
                innerConditions.Add("r2.PricePerNight <= @MaxPrice");
                parameters.Add("MaxPrice", filter.MaxPrice.Value);
            }

            if (!string.IsNullOrEmpty(filter.Type))
            {
                if (Enum.TryParse<RoomType>(filter.Type, true, out var typeEnum))
                {
                    innerConditions.Add("r2.Type = @Type");
                    parameters.Add("Type", (int)typeEnum);
                }
            }

            if (filter.MinAdults.HasValue)
            {
                innerConditions.Add("r2.AdultsCapacity >= @MinAdultsCapacity");
                parameters.Add("MinAdultsCapacity", filter.MinAdults.Value);
            }

            if (filter.MinChildren.HasValue)
            {
                innerConditions.Add("r2.ChildrenCapacity >= @MinChildrenCapacity");
                parameters.Add("MinChildrenCapacity", filter.MinChildren.Value);
            }

            var namesCount = filter.AmenityNames?.Count ?? 0;
            var names = filter.AmenityNames ?? new List<string>();

            if (namesCount > 0)
            {
                innerConditions.Add(@"(
                    SELECT COUNT(DISTINCT a2.Name)
                    FROM RoomAmenities ra2
                    INNER JOIN Amenities a2 ON ra2.AmenitiesId = a2.Id
                    WHERE ra2.RoomId = r2.Id AND a2.Name IN @Names
                ) = @NamesCount");
                parameters.Add("Names", names);
                parameters.Add("NamesCount", namesCount);
            }

            var whereClause = innerConditions.Count > 0
                ? "WHERE " + string.Join(" AND ", innerConditions)
                : string.Empty;

            var sql = $"""
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
                    {whereClause}
                    ORDER BY r2.Title
                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY
                ) AS paged
                INNER JOIN Rooms r        ON r.Id = paged.Id
                INNER JOIN Listings l     ON l.Id = r.ListingId
                LEFT JOIN  RoomAmenities ra ON ra.RoomId = r.Id
                LEFT JOIN  Amenities a    ON a.Id = ra.AmenitiesId
                ORDER BY r.Title;
                """;

            Console.WriteLine(sql);

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
                parameters,
                splitOn: "AmenityId"
            );

            return roomDictionary.Values.ToList();
        }
    }
}
