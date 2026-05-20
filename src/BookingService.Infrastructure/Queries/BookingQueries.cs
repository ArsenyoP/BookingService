using Booking.Application.DTOs.Bookings;
using Booking.Application.Interfaces.IQueries;
using Booking.Domain.Entities;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Booking.Infrastructure.Queries
{
    public class BookingQueries(string connectionString) : IBookingQueries
    {
        public async Task<IReadOnlyList<BookingResponseDto>?> GetAllPagedAsync(int page, int pageSize, CancellationToken ct = default)
        {
            using var connection = new SqlConnection(connectionString);
            var offset = (page - 1) * pageSize;

            const string sql = @"
                SELECT b.Id,
                    b.RoomId,
                    b.GuestId,
                    b.StartDate,
                    b.EndDate,
                    b.TotalNights,
                    b.PricePerNight,
                    b.TotalPrice,
                    b.AdultsCount,
                    b.ChildrenCount,
                    b.Status,
                    r.Title AS RoomTitle,
                    u.FirstName,
                    u.LastName
                    FROM Bookingss b
                    INNER JOIN Rooms r ON b.RoomId = r.Id
                    INNER JOIN Users u ON b.GuestId = u.Id
                ORDER BY 
                CASE
                    WHEN b.Status = 'Pending' THEN 1
                    WHEN b.Status = 'Confirmed' THEN 2
                    ELSE 3 
                END ASC,
                    b.StartDate DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var command = new CommandDefinition(
                sql,
                new { Offset = offset, PageSize = pageSize },
                cancellationToken: ct);

            var result = await connection.QueryAsync<BookingResponseDto>(command);

            return result.ToList().AsReadOnly();
        }

        public async Task<BookingResponseDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var connection = new SqlConnection(connectionString);

            const string sql = @"SELECT b.Id,
                        b.RoomId,
                        b.GuestId,
                        b.StartDate,
                        b.EndDate,
                        b.TotalNights,
                        b.PricePerNight,
                        b.TotalPrice,
                        b.AdultsCount,
                        b.ChildrenCount,
                        b.Status,
                        r.Title AS RoomTitle,
                        u.FirstName,
                        u.LastName
                    FROM Bookingss b
                    INNER JOIN Rooms r ON b.RoomId = r.Id
                    INNER JOIN Users u ON b.GuestId = u.Id
                        WHERE b.Id = @Id";

            var command = new CommandDefinition(
                sql,
                new { Id = id },
                cancellationToken: ct);

            var result = await connection.QueryFirstOrDefaultAsync<BookingResponseDto>(command);

            return result;

        }

        public async Task<IReadOnlyList<BookingResponseDto>?> GetByRoomPagedAsync(Guid roomId, int page, int pageSize, CancellationToken ct = default)
        {
            using var connection = new SqlConnection(connectionString);
            var offset = (page - 1) * pageSize;

            const string sql = @"
                SELECT b.Id,
                    b.RoomId,
                    b.GuestId,
                    b.StartDate,
                    b.EndDate,
                    b.TotalNights,
                    b.PricePerNight,
                    b.TotalPrice,
                    b.AdultsCount,
                    b.ChildrenCount,
                    b.Status,
                    r.Title AS RoomTitle,
                    u.FirstName,
                    u.LastName
                    FROM Bookingss b
                    INNER JOIN Rooms r ON b.RoomId = r.Id
                    INNER JOIN Users u ON b.GuestId = u.Id
                WHERE r.Id = @RoomId
                ORDER BY 
                CASE
                    WHEN b.Status = 'Pending' THEN 1
                    WHEN b.Status = 'Confirmed' THEN 2
                    ELSE 3 
                END ASC,
                    b.StartDate DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var command = new CommandDefinition(
                sql,
                new { RoomId = roomId, Offset = offset, PageSize = pageSize },
                cancellationToken: ct);

            var result = await connection.QueryAsync<BookingResponseDto>(command);

            return result.ToList().AsReadOnly();
        }

        public async Task<IReadOnlyList<BookingResponseDto>?> GetByUserPagedAsync(Guid userId, int page, int pageSize, CancellationToken ct = default)
        {
            using var connection = new SqlConnection(connectionString);
            var offset = (page - 1) * pageSize;

            const string sql = @"
                SELECT b.Id,
                    b.RoomId,
                    b.GuestId,
                    b.StartDate,
                    b.EndDate,
                    b.TotalNights,
                    b.PricePerNight,
                    b.TotalPrice,
                    b.AdultsCount,
                    b.ChildrenCount,
                    b.Status,
                    r.Title AS RoomTitle,
                    u.FirstName,
                    u.LastName
                    FROM Bookingss b
                    INNER JOIN Rooms r ON b.RoomId = r.Id
                    INNER JOIN Users u ON b.GuestId = u.Id
                WHERE u.Id = @UserId
                ORDER BY 
                CASE
                    WHEN b.Status = 'Pending' THEN 1
                    WHEN b.Status = 'Confirmed' THEN 2
                    ELSE 3 
                END ASC,
                    b.StartDate DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var command = new CommandDefinition(
                sql,
                new { UserId = userId, Offset = offset, PageSize = pageSize },
                cancellationToken: ct);

            var result = await connection.QueryAsync<BookingResponseDto>(command);

            return result.ToList().AsReadOnly();
        }

        public async Task<Bookings?> GetEntityByIdAsync(Guid id, CancellationToken ct = default)
        {
            using var connection = new SqlConnection(connectionString);
            const string sql = @"SELECT *
                FROM Bookingss b 
                WHERE b.Id=@Id";

            var command = new CommandDefinition(
                 sql,
                new { Id = id },
                cancellationToken: ct);

            var result = await connection.QueryFirstOrDefaultAsync<Bookings>(command);

            return result;
        }

        public async Task<BookingConfirmationEmailDto?> GetConfirmationEmailDataAsync(
            Guid bookingId,
            CancellationToken ct = default)
        {
            using var connection = new SqlConnection(connectionString);

            const string sql = @"
                SELECT
                    u.Email AS GuestEmail,
                    u.FirstName,
                    u.LastName,
                    r.Title AS RoomTitle,
                    b.StartDate,
                    b.EndDate,
                    b.TotalPrice
                FROM Bookingss b
                INNER JOIN Rooms r ON b.RoomId = r.Id
                INNER JOIN Users u ON b.GuestId = u.Id
                WHERE b.Id = @BookingId";

            var command = new CommandDefinition(sql, new { BookingId = bookingId }, cancellationToken: ct);

            var row = await connection.QueryFirstOrDefaultAsync<BookingConfirmationEmailRow>(command);

            if (row is null)
            {
                return null;
            }

            return new BookingConfirmationEmailDto(
                row.GuestEmail,
                $"{row.FirstName} {row.LastName}".Trim(),
                row.RoomTitle,
                row.StartDate,
                row.EndDate,
                row.TotalPrice);
        }

        public async Task<bool> IsRoomAvailableAsync(Guid roomId, DateOnly start, DateOnly end, CancellationToken ct = default)
        {
            var connection = new SqlConnection(connectionString);

            const string sql = @"
                SELECT CASE 
                    WHEN EXISTS (
                        SELECT 1 FROM Bookingss b
                        WHERE b.RoomId = @RoomId
                          AND b.Status != 'Cancelled'
                          AND b.StartDate < @End 
                          AND b.EndDate > @Start
                    ) THEN 0 ELSE 1 END";

            var command = new CommandDefinition(
            sql,
                new
                {
                    Start = start.ToDateTime(TimeOnly.MinValue),
                    End = end.ToDateTime(TimeOnly.MinValue),
                    RoomId = roomId
                },
                cancellationToken: ct);

            var result = await connection.ExecuteScalarAsync<bool>(command);
            return result;
        }

        private sealed class BookingConfirmationEmailRow
        {
            public string GuestEmail { get; init; } = string.Empty;
            public string FirstName { get; init; } = string.Empty;
            public string LastName { get; init; } = string.Empty;
            public string RoomTitle { get; init; } = string.Empty;
            public DateTime StartDate { get; init; }
            public DateTime EndDate { get; init; }
            public decimal TotalPrice { get; init; }
        }
    }
}
