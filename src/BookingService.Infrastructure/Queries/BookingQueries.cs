using Booking.Application.DTOs.Bookings;
using Booking.Application.Interfaces.IQueries;
using Booking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Infrastructure.Queries
{
    public class BookingQueries(string connectionString) : IBookingQueries
    {
        public Task<IReadOnlyList<BookingResponseDto>?> GetAllPagedAsync(int page, int pageSize, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<BookingResponseDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            throw new NotImplementedException();
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
