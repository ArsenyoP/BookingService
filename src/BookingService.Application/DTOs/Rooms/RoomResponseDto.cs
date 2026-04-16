using Booking.Application.DTOs.Amenities;
using Booking.Domain.Enums;

namespace Booking.Application.DTOs.Rooms
{
    public record RoomResponseDto
    {
        public Guid Id { get; init; }
        public string Title { get; init; }
        public string Description { get; init; }
        public RoomType Type { get; init; }
        public decimal PricePerNight { get; init; }
        public int AdultsCapacity { get; init; }
        public int ChildrenCapacity { get; init; }
        public string ListingTitle { get; init; }
        public Guid ListingId { get; init; }
        public List<AmenityDto> Amenities { get; init; } = new();
    }
}
