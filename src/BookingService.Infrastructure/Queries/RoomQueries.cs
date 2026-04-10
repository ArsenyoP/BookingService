using Booking.Application.DTOs.Rooms;
using Booking.Application.Queries;
using Booking.Domain.Entities;
using Dapper;
using Microsoft.Data.SqlClient;

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

            var result = await connection.QueryAsync<RoomResponseDto>(sql, new { Offset = offset, PageSize = pageSize });

            return result.ToList().AsReadOnly();
        }

        public async Task<RoomResponseDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            using var connection = new SqlConnection(connectionString);
            const string sql = @"SELECT  r.Id, 
                    r.Title, 
                    r.Description, 
                    r.Type, 
                    r.PricePerNight, 
                    r.AdultsCapacity, 
                    r.ChildrenCapacity, 
                    l.Title AS ListingTitle, 
                    r.ListingId
                FROM Rooms r 
                INNER JOIN Listings l ON r.ListingId=l.Id
                WHERE r.Id=@Id";

            var command = new CommandDefinition(
                sql,
                new { Id = id },
                cancellationToken: ct);

            var result = await connection.QueryFirstOrDefaultAsync<RoomResponseDto>(command);

            return result;
        }

        public async Task<IReadOnlyList<RoomResponseDto>> GetByListingIdAsync(Guid listingId, int page, int pageSize, CancellationToken ct = default)
        {
            using var connection = new SqlConnection(connectionString);
            const string sql = @"SELECT
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
                INNER JOIN Listings l ON r.ListingId=l.Id
                WHERE r.ListingId = @ListingId
            ORDER BY r.Title
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var offset = (page - 1) * pageSize;

            var command = new CommandDefinition(
                sql,
                new { ListingId = listingId, Offset = offset, PageSize = pageSize },
                cancellationToken: ct);

            var result = await connection.QueryAsync<RoomResponseDto>(command);

            return result.ToList().AsReadOnly();
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
    }
}
