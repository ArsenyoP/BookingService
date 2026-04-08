using Booking.Domain.Enums;

namespace Booking.Application.DTOs.Rooms
{
    public record RoomResponseDto(
        Guid Id,
        string Title,
        string Description,
        RoomType Type,
        decimal PricePerNight,
        int AdultsCapacity,
        int ChildrenCapacity,
        Guid ListingId);
}
