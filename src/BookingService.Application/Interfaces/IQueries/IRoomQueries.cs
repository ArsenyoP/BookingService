using Booking.Application.DTOs.Rooms;
using Booking.Application.Helpers.Room;
using Booking.Domain.Entities;


namespace Booking.Application.Queries
{
    public interface IRoomQueries
    {
        public Task<Room?> GetEntityByIdAsync(Guid id, CancellationToken ct = default);
        public Task<RoomResponseDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
        public Task<IReadOnlyList<RoomResponseDto>> GetByListingIdAsync(Guid listingId, int page, int pageSize, CancellationToken ct = default);

        public Task<IReadOnlyList<RoomResponseDto>> GetAllWithAmenitiesAsync(RoomQueryObject parametrs, CancellationToken ct = default);
    }
}
