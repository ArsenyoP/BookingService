using Booking.Application.DTOs.Rooms;
using Booking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.Queries
{
    public interface IRoomQueries
    {
        public Task<Room> GetEntityByIdAsync(Guid id, CancellationToken ct = default);
        public Task<RoomResponseDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
        public Task<IReadOnlyList<RoomResponseDto>> GetAllPagedAsync(int page, int pageSize, CancellationToken ct = default);
        public Task<IReadOnlyList<RoomResponseDto>> GetByListingIdAsync(Guid listingId, int page, int pageSize, CancellationToken ct = default);
    }
}
