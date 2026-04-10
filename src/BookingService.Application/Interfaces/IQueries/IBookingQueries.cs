using Booking.Application.DTOs.Bookings;
using Booking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.Interfaces.IQueries
{
    public interface IBookingQueries
    {
        public Task<Bookings?> GetEntityByIdAsync(Guid id, CancellationToken ct = default);
        public Task<BookingResponseDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
        public Task<IReadOnlyList<BookingResponseDto>?> GetAllPagedAsync(int page, int pageSize, CancellationToken ct = default);
        public Task<IReadOnlyList<BookingResponseDto>?> GetByRoomPagedAsync(int page, int pageSize, CancellationToken ct = default);
        public Task<IReadOnlyList<BookingResponseDto>?> GetByUserPagedAsync(int page, int pageSize, CancellationToken ct = default);
        public Task<bool> IsRoomAvailableAsync(Guid roomId, DateOnly start, DateOnly end, CancellationToken ct = default);
    }
}