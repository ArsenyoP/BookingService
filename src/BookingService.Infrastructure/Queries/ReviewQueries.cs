using Booking.Application.DTOs.Reviews;
using Booking.Application.Interfaces.IQueries;
using Booking.Domain.Entities;
using Booking.Domain.Enums;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Booking.Infrastructure.Queries
{
    public sealed class ReviewQueries(string connectionString) : IReviewQueries
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

        public async Task<ReviewResponseDto> GetById(Guid id, CancellationToken ct)
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
            WHERE r.Id = @ReviewId";

            var command = new CommandDefinition(
                sql,
                new { ReviewId = id },
                cancellationToken: ct);

            var result = await connection.QueryFirstOrDefaultAsync<ReviewResponseDto>(command);

            return result;
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

        public async Task<IReadOnlyList<ReviewResponseDto>> GetByUserId(int page, int pageSize, Guid userId, CancellationToken ct)
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
                WHERE u.Id = @UserId
                ORDER BY r.CreatedAt DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var offset = (page - 1) * pageSize;

            var command = new CommandDefinition(
                sql,
                new { UserId = userId, Offset = offset, PageSize = pageSize },
                cancellationToken: ct);

            var result = await connection.QueryAsync<ReviewResponseDto>(command);
            return result.ToList().AsReadOnly();
        }

        public async Task<bool> HasReviewByUserAsync(Guid userId, Guid targetId, CancellationToken ct)
        {
            using var connection = new SqlConnection(connectionString);

            await connection.OpenAsync(ct);

            var sql = @"SELECT CASE WHEN EXISTS( 
	            SELECT 1 FROM Reviews r
	            WHERE r.UserId = @UserId AND 
	            r.TargetId = @TargetId
            ) THEN 1 ELSE 0 END";

            var comamnd = new CommandDefinition(
                sql,
                new { UserId = userId, TargetId = targetId },
                cancellationToken: ct);

            var result = await connection.ExecuteScalarAsync<bool>(comamnd);
            return result;
        }

        public async Task<bool> HasListingBooking(Guid userId, Guid listingId, CancellationToken ct)
        {
            using var connection = new SqlConnection(connectionString);

            await connection.OpenAsync(ct);

            var sql = @"SELECT CASE WHEN EXISTS( 
	            SELECT 1 FROM Bookingss b
	            INNER JOIN Rooms r ON r.id = b.RoomId	
	            WHERE b.GuestId = @GuestId AND 
	            r.ListingId = @ListingId AND
	            (b.Status = 'Confirmed' OR b.Status = 'Completed')
            ) THEN 1 ELSE 0 END";

            var comamnd = new CommandDefinition(
                sql,
                new { GuestId = userId, ListingId = listingId },
                cancellationToken: ct);

            var result = await connection.ExecuteScalarAsync<bool>(comamnd);
            return result;
        }

        public async Task<bool> HasRoomBooking(Guid userId, Guid roomId, CancellationToken ct)
        {
            using var connection = new SqlConnection(connectionString);

            await connection.OpenAsync(ct);

            var sql = @"SELECT CASE WHEN EXISTS( 
	            SELECT 1 FROM Bookingss b
	            WHERE b.RoomId = @RoomId AND
                (b.Status = 'Confirmed' OR b.Status = 'Completed') 
            ) THEN 1 ELSE 0 END";

            var comamnd = new CommandDefinition(
                sql,
                new { UserId = userId, RoomId = roomId },
                cancellationToken: ct);

            var result = await connection.ExecuteScalarAsync<bool>(comamnd);
            return result;
        }

        public async Task AddedReviewToTarget(Guid targetId, int score, ReviewsTargetType targetType, IDbTransaction transaction, CancellationToken ct)
        {
            var connection = transaction.Connection;

            if (connection is null) throw new Exception("Connection is null");

            string tableName = targetType switch
            {
                ReviewsTargetType.Room => "Rooms",
                ReviewsTargetType.Listing => "Listings",
                _ => throw new ArgumentException("Invalid type")
            };

            var sql = $@"UPDATE {tableName}
                SET 
                    AverageRating = ((AverageRating * ReviewsCount) + @Score) / (ReviewsCount + 1),
                    ReviewsCount = ReviewsCount + 1.0
                WHERE Id = @TargetId ";

            var command = new CommandDefinition(
                sql,
                new { TargetId = targetId, Score = score },
                transaction: transaction,
                cancellationToken: ct);

            var result = await connection.ExecuteAsync(command);

        }

        public async Task<Review> GetReviewByUserAndTargetId(Guid userId, Guid targetId, CancellationToken ct)
        {
            var connection = new SqlConnection(connectionString);

            var sql = @"SELECT * FROM Reviews r
                WHERE r.UserId = @UserId AND r.TargetId = @TargetId ";

            var command = new CommandDefinition(
                sql,
                new { UserId = userId, TargetId = targetId },
                cancellationToken: ct);

            var result = await connection.QueryFirstOrDefaultAsync<Review>(command);
            return result;
        }

        public async Task RemovedReviewFromTarget(Guid targetId, int score, ReviewsTargetType targetType, IDbTransaction transaction, CancellationToken ct)
        {
            var connection = transaction.Connection;

            if (connection is null) throw new Exception("Connection is null");

            string tableName = targetType switch
            {
                ReviewsTargetType.Room => "Rooms",
                ReviewsTargetType.Listing => "Listings",
                _ => throw new ArgumentException("Invalid type")
            };

            var sql = $@"UPDATE {tableName}
                SET 
                    AverageRating = CASE 
                        WHEN ReviewsCount > 1 THEN ((AverageRating * ReviewsCount) - @Score) / (ReviewsCount - 1)
                        ELSE 0 
                    END,
                    ReviewsCount = CASE 
                        WHEN ReviewsCount > 0 THEN ReviewsCount - 1 
                        ELSE 0 
                    END
                WHERE Id = @TargetId";

            var command = new CommandDefinition(
                sql,
                new { TargetId = targetId, Score = score },
                transaction: transaction,
                cancellationToken: ct);

            await connection.ExecuteAsync(command);
        }
    }
}
