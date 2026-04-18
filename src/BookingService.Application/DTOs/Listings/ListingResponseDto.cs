using Booking.Application.DTOs.Amenities;
using Booking.Domain.Enums;

namespace Booking.Application.DTOs.Listings
{
    public record ListingResponseDto
    {
        public Guid Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public string Country { get; init; } = string.Empty;
        public string City { get; init; } = string.Empty;
        public string Street { get; init; } = string.Empty;
        public string HouseNumber { get; init; } = string.Empty;
        public int Floor { get; init; }
        public ListingType ListingType { get; init; }

        public List<AmenityDto> Amenities { get; init; } = new();
    }
}
