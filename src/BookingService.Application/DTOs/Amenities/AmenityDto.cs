using Booking.Domain.Enums;

namespace Booking.Application.DTOs.Amenities
{
    public record AmenityDto
    {
        public Guid AmenityId { get; init; }
        public string Name { get; init; }
        public AmenityCategory Category { get; init; }
    }
}
