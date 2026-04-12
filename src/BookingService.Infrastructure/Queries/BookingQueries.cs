using Booking.Application.DTOs.Bookings;
using Booking.Application.DTOs.Rooms;
using Booking.Application.Interfaces.IQueries;
using Booking.Domain.Entities;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public Task<IReadOnlyList<BookingResponseDto>?> GetByRoomPagedAsync(int page, int pageSize, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<BookingResponseDto>?> GetByUserPagedAsync(int page, int pageSize, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<Bookings?> GetEntityByIdAsync(Guid id, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsRoomAvailableAsync(Guid roomId, DateOnly start, DateOnly end, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
