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
        public string Country { get; init; } = "Unknown";
        public string City { get; init; } = "Unknown";
        public string Street { get; init; } = "Unknown";
        public string HouseNumber { get; init; } = "Unknown";
        public Guid ListingId { get; init; }
        public decimal AverageRating { get; init; }
        public int ReviewsCount { get; set; }
        public List<AmenityDto> Amenities { get; init; } = new();
    }
}
