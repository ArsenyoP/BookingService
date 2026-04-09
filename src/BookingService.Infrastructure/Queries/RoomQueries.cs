using Booking.Application.DTOs.Rooms;
using Booking.Application.Queries;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Booking.Infrastructure.Queries
{
    public class RoomQueries(string connectionString) : IRoomQueries
    {
        public async Task<IReadOnlyList<RoomResponseDto>> GetPagedAsyncListingTitle(int page, int pageSize, CancellationToken ct = default)
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

            var result = await connection.QueryAsync<RoomResponseDto>(sql, new { Offset = offset, PageSize = pageSize });

            return result.ToList();
        }
    }
}
