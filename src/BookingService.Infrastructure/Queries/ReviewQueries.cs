using Booking.Application.DTOs.Reviews;
using Booking.Application.Interfaces.IQueries;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Booking.Infrastructure.Queries
{
    public sealed class ReviewQueries(string connectionString) : IReviewQuery
    {
        public async Task<IReadOnlyList<ReviewResponseDto>> GetAll(int page, int pageSize, CancellationToken ct)
        {
            var connection = new SqlConnection(connectionString);


            var sql = @"SELECT 
	                r.Score,
	                r.Text,
	                u.UserName,
	                r.TargetType,
	                ISNULL(l.Title, rm.Title) AS TargetTitle,
	                r.CreatedAt,
	                r.IsEdited	
                FROM Reviews r
                INNER JOIN Users u ON u.Id = r.UserId
                LEFT JOIN Listings l ON l.Id = r.TargetId AND r.TargetType = 'Listing'
                LEFT JOIN Rooms rm ON rm.Id = r.TargetId AND r.TargetType = 'Room'
                ORDER BY r.CreatedAt DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY
                ";

            var offset = (page - 1) * pageSize;

            var command = new CommandDefinition(
                sql,
                new { Offset = offset, PageSize = pageSize },
                cancellationToken: ct);

            var result = await connection.QueryAsync<ReviewResponseDto>(command);
            return result.ToList().AsReadOnly();
        }

        public Task<ReviewResponseDto> GetById(Guid id, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyList<ReviewResponseDto>> GetByTargetId(int page, int pageSize, Guid targetId, CancellationToken ct)
        {
            var connection = new SqlConnection(connectionString);

            var sql = @"SELECT 
	                r.Score,
	                r.Text,
	                u.UserName,
	                r.TargetType,
	                ISNULL(l.Title, rm.Title) AS TargetTitle,
	                r.CreatedAt,
	                r.IsEdited	
                FROM Reviews r
                INNER JOIN Users u ON u.Id = r.UserId
                LEFT JOIN Listings l ON l.Id = r.TargetId AND r.TargetType = 'Listing'
                LEFT JOIN Rooms rm ON rm.Id = r.TargetId AND r.TargetType = 'Room'
                WHERE r.TargetId = @TargetId
                ORDER BY r.CreatedAt DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var offset = (page - 1) * pageSize;

            var command = new CommandDefinition(
                sql,
                new { TargetId = targetId, Offset = offset, PageSize = pageSize },
                cancellationToken: ct);

            var result = await connection.QueryAsync<ReviewResponseDto>(command);
            return result.ToList().AsReadOnly();
        }

        public Task<IReadOnlyList<ReviewResponseDto>> GetByUserId(int page, int pageSize, Guid userId, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
